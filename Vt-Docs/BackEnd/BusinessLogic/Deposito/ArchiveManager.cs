using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Deposito;
using log4net;

namespace BusinessLogic.Deposito
{
    public partial class ArchiveManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(ArchiveManager));
        public ArchiveManager() { }

        #region ARCHIVE_Disposal_TMP_Update

        /// <summary>
        /// Update tabelle temporanee scarto
        /// </summary>
        /// <param name="Disposal_ID"></param>
        /// <param name="listSystemID"></param>
        /// <returns></returns>
        public static Boolean Update_ARCHIVE_TempProjectDisposal(Int32 Disposal_ID, string listProjectsID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean updateSuccess = depositoDB.Update_ARCHIVE_TempProjectDisposal(Disposal_ID, listProjectsID);
                return updateSuccess;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: Update_ARCHIVE_TempProjectDisposal", e);
                return false;
            }
        }

        /// <summary>
        /// Update tabelle temporanee scarto
        /// </summary>
        /// <param name="Disposal_ID"></param>
        /// <param name="listSystemID"></param>
        /// <returns></returns>
        public static Boolean Update_ARCHIVE_TempProfileDisposal(Int32 Disposal_ID, string listProfilesID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean updateSuccess = depositoDB.Update_ARCHIVE_TempProfileDisposal(Disposal_ID, listProfilesID);
                return updateSuccess;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: Update_ARCHIVE_TempProfileDisposal", e);
                return false;
            }
        }

        #endregion

        #region ARCHIVE_JOB_Disposal
        /// <summary>
        /// Returns an instance of List<ARCHIVE_DisposalForSearch>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferForSearch. </results>
        public static List<ARCHIVE_DisposalForSearch> GetAllARCHIVE_DisposalFilterForSearch(String st_indefinizione, String st_analisicompletata, String st_proposto,
                                                                               String st_approvato, String st_inesecuzione, String st_effettuato, String st_inerrore, Int32 finger)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_DisposalForSearch> DisposalSearch = depositoDB.GetAllARCHIVE_DisposalForSearch(st_indefinizione, st_analisicompletata, st_proposto,
                                                                                st_approvato, st_inesecuzione, st_effettuato, st_inerrore, finger);
                return DisposalSearch;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetAllARCHIVE_ARCHIVE_DisposalForSearch", e);
                return null;
            }
        }

        /// <summary>
        /// Persists a new instance of ARCHIVE_JOB_Disposal to the database.
        /// </summary>
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public static void InsertARCHIVE_JOB_Disposal(Int32 Disposal_ID, Int32 jobType_ID, ref Int32 system_ID)
        {
            try
            {
                system_ID = 0;
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                depositoDB.InsertARCHIVE_JOB_Disposal(Disposal_ID, jobType_ID, ref system_ID);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: InsertARCHIVE_JOB_Disposal", e);
            }
        }


        /// <summary>
        /// Returns an instance of List<ARCHIVE_JOB_Disposal>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_Disposal. </results>
        public static List<ARCHIVE_JOB_Disposal> GetARCHIVE_JOB_DisposalByDisposal_ID(Int32 disposal_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_JOB_Disposal> Disposal = depositoDB.GetARCHIVE_JOB_DisposalByDisposal_ID(disposal_ID);
                return Disposal;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GGetARCHIVE_JOB_DisposalByDisposal_ID", e);
                return null;
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
        public static List<ARCHIVE_View_Documents_Policy> GetAllARCHIVE_View_Documents_Disposal(Int32 Disposal_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_View_Documents_Policy> disposal_view_doc_DG = depositoDB.GetAllARCHIVE_View_Documents_Disposal(Disposal_ID);
                return disposal_view_doc_DG;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetAllARCHIVE_View_Documents_Disposal", e);
                return null;
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
        public static List<ARCHIVE_View_Projects_Policy> GetAllARCHIVE_View_Projects_Disposal(Int32 Disposal_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_View_Projects_Policy> disposal_view_doc_DG = depositoDB.GetAllARCHIVE_View_Projects_Disposal(Disposal_ID);
                return disposal_view_doc_DG;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetAllARCHIVE_View_Projects_Disposal", e);
                return null;
            }
        }

        #endregion

        #region ARCHIVE_Disposal

        /// <summary>
        /// Returns an instance of List<ARCHIVE_Disposal>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_Disposal. </results>
        public static List<ARCHIVE_Disposal> GetARCHIVE_DisposalBySystem_ID(Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_Disposal> Disposal = depositoDB.GetARCHIVE_DisposalBySystem_ID(system_ID);
                return Disposal;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetARCHIVE_DisposalBySystem_ID", e);
                return null;
            }
        }
        /// <summary>
        /// Returns all instances of ARCHIVE_Disposal.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_Disposal. </results>
        public static List<ARCHIVE_Disposal> GetAllARCHIVE_Disposal()
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_Disposal> Disposal = depositoDB.GetAllARCHIVE_Disposal();
                return Disposal;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetAllARCHIVE_Disposal", e);
                return null;
            }
        }
        /// <summary>
        /// Deletes an instance of ARCHIVE_Disposal based on System_ID.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public static Boolean DeleteARCHIVE_Disposal(Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean DeleteSucceded = depositoDB.DeleteARCHIVE_Disposal(system_ID);
                return DeleteSucceded;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: DeleteARCHIVE_Disposal", e);
                return false;
            }
        }
        /// <summary>
        /// Updates an instance of ARCHIVE_Disposal.
        /// </summary>
        /// <param name="note">This is not a required field. </param>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public static Boolean UpdateARCHIVE_Disposal(String description, String note, Int32 system_ID, Int32 id_Amministrazione)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean updateSuccess = depositoDB.UpdateARCHIVE_Disposal(description, note, system_ID, id_Amministrazione);
                return updateSuccess;
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
        public static Boolean UpdateARCHIVE_DisposalOriginal(String description, String description_Original, String note,
                                                            String note_Original, Int32 system_ID, Int32 id_Amministrazione, Int32 id_Amministrazione_Original)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean updateSuccess = depositoDB.UpdateARCHIVE_Disposal(description, description_Original, note, note_Original, id_Amministrazione, id_Amministrazione_Original, system_ID);
                return updateSuccess;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: UpdateARCHIVE_Disposal", e);
                return false;
            }
        }
        /// <summary>
        /// Persists a new instance of ARCHIVE_Disposal to the database.
        /// </summary>
        /// <param name="note">This is not a required field. </param>
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public static void InsertARCHIVE_Disposal(String description, String note, ref Int32 system_ID, Int32 iD_Amministrazione, Int32 disposalStateType_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                depositoDB.InsertARCHIVE_Disposal(description, note, ref system_ID, iD_Amministrazione, disposalStateType_ID);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: UpdateARCHIVE_Disposal", e);
            }
        }

        #endregion

        #region ARCHIVE_DisposalState

        /// <summary>
        /// Returns all instances of ARCHIVE_DisposalState.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_DisposalState. </results>
        public static List<ARCHIVE_DisposalState> GetAllARCHIVE_DisposalState()
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_DisposalState> archive_states = depositoDB.GetAllARCHIVE_DisposalState();
                return archive_states;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetAll", e);
                return null;
            }
        }
        /// <summary>
        /// Returns an instance of List<ARCHIVE_DisposalState>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_DisposalState. </results>
        public static List<ARCHIVE_DisposalState> GetARCHIVE_DisposalStateBySystem_ID(Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_DisposalState> archive_state = depositoDB.GetARCHIVE_DisposalStateBySystem_ID(system_ID);
                return archive_state;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetARCHIVE_DisposalStateBySystem_ID", e);
                return null;
            }
        }
        /// <summary>
        /// Relates ARCHIVE_Disposal to ARCHIVE_DisposalState.
        /// Returns a collection of ARCHIVE_DisposalState based on the following criteria: Disposal_ID.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_DisposalState. </results>
        public static List<ARCHIVE_DisposalState> GetARCHIVE_DisposalStateByDisposal_ID(Int32 disposal_ID)
        {

            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_DisposalState> archive_state = depositoDB.GetARCHIVE_DisposalStateByDisposal_ID(disposal_ID);
                return archive_state;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetARCHIVE_DisposalStateByDisposal_ID", e);
                return null;
            }
        }
        /// <summary>
        /// Returns a collection of ARCHIVE_DisposalState based on the following criteria: DisposalStateType_ID.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_DisposalState. </results>
        public static List<ARCHIVE_DisposalState> GetARCHIVE_DisposalStateByDisposalStateType_ID(Int32 disposalStateType_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_DisposalState> archive_state = depositoDB.GetARCHIVE_DisposalStateByDisposalStateType_ID(disposalStateType_ID);
                return archive_state;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetARCHIVE_DisposalStateByDisposalStateType_ID", e);
                return null;
            }
        }
        /// <summary>
        /// Deletes an instance of ARCHIVE_DisposalState based on System_ID.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public static Boolean DeleteARCHIVE_DisposalState(Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                bool deleteSuccess = depositoDB.DeleteARCHIVE_DisposalState(system_ID);
                return deleteSuccess;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: DeleteARCHIVE_DisposalState", e);
                return false;
            }
        }
        /// <summary>
        /// Deletes an instance of ARCHIVE_DisposalState based on Disposal_ID.
        /// </summary>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public static Boolean DeleteARCHIVE_DisposalStateByDisposal_ID(Int32 disposal_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                bool deleteSuccess = depositoDB.DeleteARCHIVE_DisposalStateByDisposal_ID(disposal_ID);
                return deleteSuccess;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: DeleteARCHIVE_DisposalStateByDisposal_ID", e);
                return false;
            }
        }
        /// <summary>
        /// Updates an instance of ARCHIVE_DisposalState.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public static Boolean UpdateARCHIVE_DisposalState(Int32 disposal_ID, Int32 disposalStateType_ID, Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean updateSuccess = depositoDB.UpdateARCHIVE_DisposalState(disposal_ID, disposalStateType_ID, system_ID);
                return updateSuccess;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: UpdateARCHIVE_DisposalState", e);
                return false;
            }
        }
        /// <summary>
        /// Updates an instance of ARCHIVE_DisposalState if the original data has not changed.
        /// </summary>
        /// <param name="disposal_ID_Original">This field is used for optimistic concurrency management. It should contain the original value of 'disposal_ID_Original'. </param>
        /// <param name="disposalStateType_ID_Original">This field is used for optimistic concurrency management. It should contain the original value of 'disposalStateType_ID_Original'. </param>
        /// <param name="dateTime_Original">This field is used for optimistic concurrency management. It should contain the original value of 'dateTime_Original'. </param>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public static Boolean UpdateARCHIVE_DisposalState(Int32 disposal_ID, Int32 disposal_ID_Original, Int32 disposalStateType_ID, Int32 disposalStateType_ID_Original, Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean updateSuccess = depositoDB.UpdateARCHIVE_DisposalState(disposal_ID, disposalStateType_ID, system_ID);
                return updateSuccess;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: UpdateARCHIVE_DisposalState", e);
                return false;
            }
        }
        /// <summary>
        /// Persists a new instance of ARCHIVE_DisposalState to the database.
        /// </summary>
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public static void InsertARCHIVE_DisposalState(Int32 disposal_ID, Int32 disposalStateType_ID, ref Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                depositoDB.InsertARCHIVE_DisposalState(disposal_ID, disposalStateType_ID, ref system_ID);
            }
            catch (Exception e)
            {
                logger.Debug("Errore - metodo: InsertARCHIVE_DisposalStateType", e);
            }
        }

        #endregion

        #region ARCHIVE_DisposalStateType

        /// <summary>
        /// Returns all instances of ARCHIVE_DisposalStateType.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_DisposalStateType. </results>
        public static List<ARCHIVE_DisposalStateType> GetAllARCHIVE_DisposalStateType()
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_DisposalStateType> archive_states_types = depositoDB.GetAllARCHIVE_DisposalStateType();
                return archive_states_types;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetAllARCHIVE_DisposalStateType", e);
                return null;
            }
        }
        /// <summary>
        /// Returns an instance of List<ARCHIVE_DisposalStateType>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_DisposalStateType. </results>
        public static List<ARCHIVE_DisposalStateType> GetARCHIVE_DisposalStateTypeBySystem_ID(Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_DisposalStateType> archive_states_types = depositoDB.GetARCHIVE_DisposalStateTypeBySystem_ID(system_ID);
                return archive_states_types;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetARCHIVE_DisposalStateTypeBySystem_ID", e);
                return null;
            }
        }
        /// <summary>
        /// Deletes an instance of ARCHIVE_DisposalStateType based on System_ID.
        /// </summary>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public static Boolean DeleteARCHIVE_DisposalStateType(Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean delSuccess = depositoDB.DeleteARCHIVE_DisposalStateType(system_ID);
                return delSuccess;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: DeleteARCHIVE_DisposalStateType", e);
                return false;
            }
        }
        /// <summary>
        /// Deletes all instances of ARCHIVE_DisposalStateType.
        /// </summary>
        /// <results>The number of items deleted. </results>
        public static Int32 DeleteAllARCHIVE_DisposalStateType()
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                int rowEffected = depositoDB.DeleteAllARCHIVE_DisposalStateType();
                return rowEffected;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: DeleteAllARCHIVE_DisposalStateType", e);
                return -1;
            }
        }
        /// <summary>
        /// Updates an instance of ARCHIVE_DisposalStateType.
        /// </summary>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public static Boolean UpdateARCHIVE_DisposalStateType(String name, Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean updateSuccess = depositoDB.UpdateARCHIVE_DisposalStateType(name, system_ID);
                return updateSuccess;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: UpdateARCHIVE_DisposalStateType", e);
                return false;
            }
        }
        /// <summary>
        /// Updates an instance of ARCHIVE_DisposalStateType if the original data has not changed.
        /// </summary>
        /// <param name="name_Original">This field is used for optimistic concurrency management. It should contain the original value of 'name_Original'. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public static Boolean UpdateARCHIVE_DisposalStateType(String name, String name_Original, Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean updateSuccess = depositoDB.UpdateARCHIVE_DisposalStateType(name, name_Original, system_ID);
                return updateSuccess;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: UpdateARCHIVE_DisposalStateType", e);
                return false;
            }
        }
        /// <summary>
        /// Persists a new instance of ARCHIVE_DisposalStateType to the database.
        /// </summary>
        public static Boolean InsertARCHIVE_DisposalStateType(Int32 system_ID, String name)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean updateSuccess = depositoDB.InsertARCHIVE_DisposalStateType(system_ID, name);
                return updateSuccess;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: InsertARCHIVE_DisposalStateType", e);
                return false;
            }
        }

        #endregion

        #region ARCHIVE_RESULT

        public static List<ARCHIVE_AUTH_Authorization_Result> GetAllARCHIVE_Authorization_Result()
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_AUTH_Authorization_Result> grid_document = null;
                return grid_document;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetAllARCHIVE__Authorization_Result", e);
                return null;
            }
        }

        #endregion

        #region ARCHIVE_AUTH

        /// <summary>
        /// Returns an instance of List<ARCHIVE_Transfer>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_Transfer. </results>
        public static List<ARCHIVE_AUTH_Authorization> GetARCHIVE_AutorizationBySystem_ID(Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_AUTH_Authorization> Transfer = depositoDB.GetARCHIVE_AutorizationBySystem_ID(system_ID);
                return Transfer;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetARCHIVE_AutorizationBySystem_ID", e);
                return null;
            }
        }

        /// <summary>
        /// Returns all instance of List<ARCHIVE_AUTH_Authorization>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_AUTH_Authorization. </results>
        public static List<ARCHIVE_AUTH_Authorization> GetALLARCHIVE_Autorizations()
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_AUTH_Authorization> Transfer = depositoDB.GetALLARCHIVE_Autorizations();
                return Transfer;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetALLARCHIVE_Autorizations", e);
                return null;
            }
        }

        /// <summary>
        /// Deletes an instance of ARCHIVE_AUTH_Authorization based on System_ID.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public static Boolean DeleteARCHIVE_Autorizations_BySystem_ID(Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean DeleteSucceded = depositoDB.DeleteARCHIVE_Autorizations_BySystem_ID(system_ID);
                return DeleteSucceded;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: DeleteARCHIVE_Autorizations_BySystem_ID", e);
                return false;
            }
        }

        /// <summary>
        /// Persists a new instance of ARCHIVE_Transfer to the database.
        /// </summary>
        /// <param name="note">This is not a required field. </param>
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public static void InsertARCHIVE_Authorization(Int32 People_ID, String StartDate, String EndDate, String note, String profileList,
                                                        String projectList, ref Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                depositoDB.InsertARCHIVE_Authorization(People_ID, StartDate, EndDate, note, profileList, projectList, ref system_ID);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: InsertARCHIVE_Authorization", e);
            }
        }

        /// <summary>
        /// Updates an instance of ARCHIVE_AUTH_Authorization.
        /// </summary>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public static Boolean UpdateARCHIVE_Authorization(Int32 People_ID, String StartDate, String EndDate, String note, String profileList,
                                                        String projectList, ref Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean updateSuccess = depositoDB.UpdateARCHIVE_Authorization(People_ID, StartDate, EndDate, note, profileList, projectList, ref system_ID);
                return updateSuccess;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: UpdateARCHIVE_Authorization", e);
                return false;
            }
        }

        public static List<ARCHIVE_AUTH_grid_document> GetAllARCHIVE_GridDocument()
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_AUTH_grid_document> grid_document = null;
                return grid_document;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetAllARCHIVE_GridDocument", e);
                return null;
            }
        }

        public static List<ARCHIVE_AUTH_grid_project> GetAllARCHIVE_GridProject()
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_AUTH_grid_project> grid_project = null;
                return grid_project;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: ARCHIVE_AUTH_grid_project", e);
                return null;
            }
        }


        #endregion

        #region ARCHIVE_AUTH_OBJECT
        /// <summary>
        /// Returns an instance of List<ARCHIVE_AUTH_AuthorizedObject>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_Transfer. </results>
        public static List<ARCHIVE_AUTH_AuthorizedObject> GetARCHIVE_AutorizedObjectBySystem_ID(Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_AUTH_AuthorizedObject> Transfer = depositoDB.GetARCHIVE_AutorizedObjectBySystem_ID(system_ID);
                return Transfer;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetARCHIVE_AutorizedObjectBySystem_ID", e);
                return null;
            }
        }

        /// <summary>
        /// Returns all instance of List<ARCHIVE_AUTH_AuthorizedObject>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_AUTH_Authorization. </results>
        public static List<ARCHIVE_AUTH_AuthorizedObject> GetALLARCHIVE_AutorizedObject()
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_AUTH_AuthorizedObject> Transfer = depositoDB.GetALLARCHIVE_AutorizedObject();
                return Transfer;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetALLARCHIVE_AutorizedObject", e);
                return null;
            }
        }

        /// <summary>
        /// Deletes an instance of ARCHIVE_AUTH_AuthorizedObject based on System_ID.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public static Boolean DeleteARCHIVE_AutorizedObject_BySystem_ID(Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean DeleteSucceded = depositoDB.DeleteARCHIVE_AutorizedObject_BySystem_ID(system_ID);
                return DeleteSucceded;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: DeleteARCHIVE_AutorizedObject_BySystem_ID", e);
                return false;
            }
        }

        /// <summary>
        /// Persists a new instance of ARCHIVE_AUTH_AuthorizedObject to the database.
        /// </summary>
        /// <param name="note">This is not a required field. </param>
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public static void InsertARCHIVE_AutorizedObject(Int32 Authorization_ID, Int32 Project_ID, Int32 profile_ID, ref Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                depositoDB.InsertARCHIVE_AutorizedObject(Authorization_ID, Project_ID, profile_ID, ref system_ID);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: InsertARCHIVE_AutorizedObject", e);
            }
        }

        /// <summary>
        /// Updates an instance of ARCHIVE_AUTH_Authorization.
        /// </summary>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public static Boolean UpdateARCHIVE_AutorizedObject(Int32 Authorization_ID, Int32 Project_ID, Int32 profile_ID, ref Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean updateSuccess = depositoDB.UpdateARCHIVE_AutorizedObject(Authorization_ID, Project_ID, profile_ID, ref system_ID);
                return updateSuccess;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: UpdateARCHIVE_AutorizedObject", e);
                return false;
            }
        }

        #endregion

        #region  ARCHIVE_LOG_TransferAndPolicy
        /// <summary>
        /// Returns an instance of List<ARCHIVE_LOG_TransferAndPolicy>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_Transfer. </results>
        public static List<ARCHIVE_LOG_TransferAndPolicy> GetARCHIVE_LOG_TransferAndPolicy(String ListaVersamentoIDANDPolicyID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_LOG_TransferAndPolicy> Logs = depositoDB.GetARCHIVE_LOG_TransferAndPolicy(ListaVersamentoIDANDPolicyID);
                return Logs;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetARCHIVE_JOB_TransferByTransfer_ID", e);
                return null;
            }
        }
        #endregion

        #region ARCHIVE_JOB_Transfer

        /// <summary>
        /// Persists a new instance of ARCHIVE_JOB_Transfer to the database.
        /// </summary>
        /// <param name="note">This is not a required field. </param>
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public static void InsertARCHIVE_JOB_Transfer(Int32 transfer_ID, Int32 jobType_ID, ref Int32 system_ID)
        {
            try
            {
                system_ID = 0;
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                depositoDB.InsertARCHIVE_JOB_Transfer(transfer_ID, jobType_ID, ref system_ID);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: InsertARCHIVE_JOB_Transfer", e);
            }
        }

        /// <summary>
        /// Returns an instance of List<ARCHIVE_JOB_Transfer>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_Transfer. </results>
        public static List<ARCHIVE_JOB_Transfer> GetARCHIVE_JOB_TransferByTransfer_ID(Int32 transfer_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_JOB_Transfer> Transfer = depositoDB.GetARCHIVE_JOB_TransferByTransfer_ID(transfer_ID);
                return Transfer;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GGetARCHIVE_JOB_TransferByTransfer_ID", e);
                return null;
            }
        }

        #endregion

        #region ARCHIVE_JOB_TransferPolicy
        /// <summary>
        /// Returns an instance of List<ARCHIVE_JOB_TransferPolicy>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_JOB_TransferPolicy, related to a transfer </results>
        public static List<ARCHIVE_JOB_TransferPolicy> GetARCHIVE_JOB_TransferPolicyByTransfer_ID(Int32 transfer_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_JOB_TransferPolicy> Transfer = depositoDB.GetARCHIVE_JOB_TransferPolicyByTransfer_ID(transfer_ID);
                return Transfer;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GGetARCHIVE_JOB_TransferByTransfer_ID", e);
                return null;
            }
        }

        /// <summary>
        /// Returns an instance of List<ARCHIVE_JOB_TransferPolicy>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_JOB_TransferPolicy. </results>
        public static List<ARCHIVE_JOB_TransferPolicy> GetARCHIVE_JOB_TransferPolicyByTransferPolicy_ID(Int32 transferPolicy_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_JOB_TransferPolicy> Transfer = depositoDB.GetARCHIVE_JOB_TransferPolicyByTransferPolicy_ID(transferPolicy_ID);
                return Transfer;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GGetARCHIVE_JOB_TransferByTransferPolicy_ID", e);
                return null;
            }
        }

        /// <summary>
        /// Persists a new instance of ARCHIVE_JOB_TransferPolicy to the database.
        /// </summary>
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public static void InsertARCHIVE_JOB_TransferPolicy(Int32 transferPolicy_ID, Int32 jobType_ID, ref Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                depositoDB.InsertARCHIVE_JOB_TransferPolicy(transferPolicy_ID, jobType_ID, ref system_ID);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: InsertARCHIVE_JOB_TransferPolicy", e);
            }
        }

        #endregion

        #region Utility di Search & porting Transfer Policy

        /// <summary>
        /// Chiamata per lo start della ricerca dei documenti in base alla policy che abbiamo inserito.
        /// </summary>
        /// <param name="system_ID"></param>
        /// <returns></returns>
        public static bool StartSearchForTransferPolicy(Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                depositoDB.StartSearchForTransferPolicy(system_ID);
                return true;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: sp_ARCHIVE_BE_StartSearchForPolicy", e);
                return false;
            }
        }

        /// <summary>
        /// avvia la ricerca di una lista di policy rappresentanta da una stringa di id policy passata per parametro
        /// </summary>
        /// <param name="system_ID"></param>
        /// <returns></returns>
        public static bool StartSearchForTransferPolicyList(string ListSystemID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                depositoDB.StartSearchForTransferPolicyList(ListSystemID);
                return true;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: sp_ARCHIVE_BE_StartSearchForPolicyList", e);
                return false;
            }
        }



        /// <summary>
        /// avvia l'analisi di una lista di policy rappresentanta da una stringa di id policy passata per parametro
        /// </summary>
        /// <param name="system_ID"></param>
        /// <returns></returns>
        public static bool StartAnalysisForTransferPolicyList(string ListSystemID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                depositoDB.StartAnalysisForTransferPolicyList(ListSystemID);
                return true;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: sp_ARCHIVE_BE_StartAnalysisForPolicyList", e);
                return false;
            }
        }


        /// <summary>
        /// avvia la ricerca asincrona di una lista di policy rappresentanta da una stringa di id policy passata per parametro
        /// </summary>
        /// <param name="system_ID"></param>
        /// <returns>una lista con gli id delle policy non schedulate</returns>
        public static List<Int32> StartAsyncSearchForTransferPolicyList(string ListSystemID)
        {
            try
            {
                List<Int32> result = new List<Int32>();
                Int32 system_id = 0;
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                foreach (string id in ListSystemID.Split(','))
                {
                    depositoDB.InsertARCHIVE_JOB_TransferPolicy(Int32.Parse(id), 1, ref system_id);
                    if (system_id == 0)
                        result.Add(Int32.Parse(id));
                }

                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: sp_ARCHIVE_BE_StartAsyncSearchForPolicyList", e);
                return null;
            }
        }

        /// <summary>
        /// avvia l'analisi asincrona di una lista di policy rappresentanta da una stringa di id policy passata per parametro
        /// </summary>
        /// <param name="system_ID"></param>
        /// <returns>una lista con gli id delle policy non schedulate</returns>
        public static List<Int32> StartAsyncAnalysisForTransferPolicyList(string ListSystemID)
        {
            try
            {
                List<Int32> result = new List<Int32>();
                Int32 system_id = 0;
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                foreach (string id in ListSystemID.Split(','))
                {
                    depositoDB.InsertARCHIVE_JOB_TransferPolicy(Int32.Parse(id), 2, ref system_id);
                    if (system_id == 0)
                        result.Add(Int32.Parse(id));
                }

                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: sp_ARCHIVE_BE_StartAsyncAnalysisForPolicyList", e);
                return null;
            }
        }


        /// <summary>
        /// Analisi del Trasferimento
        /// </summary>
        /// <param name="system_ID">ID TRANSFER</param>
        /// <returns></returns>
        public static bool StartAnalysisForTransfer(Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                depositoDB.StartAnalysisForTransfer(system_ID);
                return true;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: StartAnalysisForTransfer", e);
                return false;
            }
        }

        /// <summary>
        /// Returns an instance of List<ARCHIVE_TransferForSearch>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferForSearch. </results>
        public static List<ARCHIVE_TransferForSearch> GetAllARCHIVE_TransferFilterForSearch(String st_indefinizione, String st_analisicompletata, String st_proposto,
                                                                               String st_approvato, String st_inesecuzione, String st_effettuato, String st_inerrore, Int32 finger)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_TransferForSearch> TransferSearch = depositoDB.GetAllARCHIVE_TransferFilterForSearch(st_indefinizione, st_analisicompletata, st_proposto,
                                                                                st_approvato, st_inesecuzione, st_effettuato, st_inerrore, finger);
                return TransferSearch;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetAllARCHIVE_TransferFilterForSearch", e);
                return null;
            }
        }

        #endregion

        #region ARCHIVE_Transfer_Policy
        /// <summary>
        /// Deletes many instance of ARCHIVE_TransferPolicy based on System_ID.
        /// </summary>
        /// <param name="ListSystem_ID">The database automatically generates this value. </param>
        ///
        public static Boolean DeleteARCHIVE_TransferPolicyList(string ListSystemId)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean DeleteSucceded = true;
                foreach (string id in ListSystemId.Split(','))
                {
                    DeleteSucceded = DeleteSucceded && depositoDB.DeleteARCHIVE_TransferPolicy(int.Parse(id));
                }
                return DeleteSucceded;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: DeleteARCHIVE_TransferPolicyList", e);
                return false;
            }

        }
        /// <summary>
        /// Returns an instance of List<ARCHIVE_TransferPolicy>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferPolicy. </results>
        public static List<ARCHIVE_TransferPolicy> GetARCHIVE_TransferPolicyBySystem_ID(Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_TransferPolicy> TransferPolicy = depositoDB.GetARCHIVE_TransferPolicyBySystem_ID(system_ID);
                return TransferPolicy;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetARCHIVE_TransferPolicyBySystem_ID", e);
                return null;
            }
        }
        /// <summary>
        /// Returns all instances of ARCHIVE_TransferPolicy.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferPolicy. </results>
        public static List<ARCHIVE_TransferPolicy> GetAllARCHIVE_TransferPolicy()
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_TransferPolicy> TransferPolicy = depositoDB.GetAllARCHIVE_TransferPolicy();
                return TransferPolicy;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetAllARCHIVE_TransferPolicy", e);
                return null;
            }
        }
        /// <summary>
        /// Relates ARCHIVE_TransferPolicyType to ARCHIVE_TransferPolicy.
        /// Returns a collection of ARCHIVE_TransferPolicy based on the following criteria: TransferPolicyType_ID.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferPolicy. </results>
        public static List<ARCHIVE_TransferPolicy> GetARCHIVE_TransferPolicyByTransferPolicyType_ID(Int32 transferPolicyType_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_TransferPolicy> TransferPolicy = depositoDB.GetARCHIVE_TransferPolicyByTransferPolicyType_ID(transferPolicyType_ID);
                return TransferPolicy;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetAllARCHIVE_TransferPolicy", e);
                return null;
            }
        }
        /// <summary>
        /// Returns a collection of ARCHIVE_TransferPolicy based on the following criteria: Transfer_ID.
        /// </summary>
        /// <param name="transfer_ID">This is not a required field. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferPolicy. </results>
        public static List<ARCHIVE_TransferPolicy> GetARCHIVE_TransferPolicyByTransfer_ID(Int32? transfer_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_TransferPolicy> TransferPolicy = depositoDB.GetARCHIVE_TransferPolicyByTransfer_ID(transfer_ID);
                return TransferPolicy;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetAllARCHIVE_TransferPolicy", e);
                return null;
            }
        }
        /// <summary>
        /// Deletes an instance of ARCHIVE_TransferPolicy based on System_ID.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public static Boolean DeleteARCHIVE_TransferPolicy(Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean DeleteSucceded = depositoDB.DeleteARCHIVE_TransferPolicy(system_ID);
                return DeleteSucceded;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: DeleteARCHIVE_TransferPolicy", e);
                return false;
            }

        }

        ///// <summary>
        ///// Deletes many instance of ARCHIVE_TransferPolicy based on System_ID.
        ///// </summary>
        ///// <param name="ListSystem_ID">The database automatically generates this value. </param>
        /////
        //public static Boolean DeleteARCHIVE_TransferPolicyList(string ListSystemId)
        //{
        //    try
        //    {
        //        DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
        //        Boolean DeleteSucceded = true;
        //        foreach (string id in ListSystemId.Split(','))
        //        {
        //            DeleteSucceded = DeleteSucceded && depositoDB.DeleteARCHIVE_TransferPolicy(int.Parse(id));
        //        }
        //        return DeleteSucceded;
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Debug("Errore in DiagrammiStato  - metodo: DeleteARCHIVE_TransferPolicyList", e);
        //        return false;
        //    }

        //}
        /// <summary>
        /// Deletes all instances of ARCHIVE_TransferPolicy.
        /// </summary>
        /// <results>The number of items deleted. </results>
        public static Int32 DeleteAllARCHIVE_TransferPolicy()
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                int DeleteSucceded = depositoDB.DeleteAllARCHIVE_TransferPolicy();
                return DeleteSucceded;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: DeleteARCHIVE_Transfer", e);
                return -1;
            }
        }
        /// <summary>
        /// Deletes an instance of ARCHIVE_TransferPolicy based on TransferPolicyType_ID.
        /// </summary>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public static Boolean DeleteARCHIVE_TransferPolicyByTransferPolicyType_ID(Int32 transferPolicyType_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean DeleteSucceded = depositoDB.DeleteARCHIVE_TransferPolicyByTransferPolicyType_ID(transferPolicyType_ID);
                return DeleteSucceded;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: DeleteARCHIVE_TransferPolicyByTransferPolicyType_ID", e);
                return false;
            }
        }
        /// <summary>
        /// Updates an instance of ARCHIVE_TransferPolicy.
        /// </summary>
        /// <param name="aRCHIVE_TransferPolicy">The instance of aRCHIVE_TransferPolicy to persist. </param>
        /// <param name="aRCHIVE_TransferPolicy">The original instance of aRCHIVE_TransferPolicy. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public static Boolean UpdateARCHIVE_TransferPolicy(ARCHIVE_TransferPolicy aRCHIVE_TransferPolicy)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean UpdateSucceded = depositoDB.UpdateARCHIVE_TransferPolicy(aRCHIVE_TransferPolicy.Description, aRCHIVE_TransferPolicy.Enabled,
                    aRCHIVE_TransferPolicy.Transfer_ID, aRCHIVE_TransferPolicy.TransferPolicyType_ID, aRCHIVE_TransferPolicy.TransferPolicyState_ID, aRCHIVE_TransferPolicy.Registro_ID,
                    aRCHIVE_TransferPolicy.UO_ID, aRCHIVE_TransferPolicy.IncludiSottoalberoUO, aRCHIVE_TransferPolicy.Tipologia_ID,
                    aRCHIVE_TransferPolicy.Titolario_ID, aRCHIVE_TransferPolicy.ClasseTitolario, aRCHIVE_TransferPolicy.IncludiSottoalberoClasseTit,
                    aRCHIVE_TransferPolicy.AnnoCreazioneDa, aRCHIVE_TransferPolicy.AnnoCreazioneA, aRCHIVE_TransferPolicy.AnnoProtocollazioneDa,
                    aRCHIVE_TransferPolicy.AnnoProtocollazioneA, aRCHIVE_TransferPolicy.AnnoChiusuraDa, aRCHIVE_TransferPolicy.AnnoChiusuraA, aRCHIVE_TransferPolicy.System_ID);
                return UpdateSucceded;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: UpdateARCHIVE_TransferPolicy", e);
                return false;
            }
        }



        /// <summary>
        /// Updates an instance of ARCHIVE_TransferPolicy.
        /// </summary>
        /// <param name="aRCHIVE_TransferPolicy">The instance of aRCHIVE_TransferPolicy to persist. </param>
        /// <param name="aRCHIVE_TransferPolicy">The original instance of aRCHIVE_TransferPolicy. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public static Boolean UpdateARCHIVE_TransferPolicy(ARCHIVE_TransferPolicy aRCHIVE_TransferPolicy, ARCHIVE_TransferPolicy aRCHIVE_TransferPolicy_Original)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean UpdateSucceded = depositoDB.UpdateARCHIVE_TransferPolicy(
                        aRCHIVE_TransferPolicy.Description,
                        aRCHIVE_TransferPolicy_Original.Description,
                        aRCHIVE_TransferPolicy.Enabled,
                        aRCHIVE_TransferPolicy_Original.Enabled,
                        aRCHIVE_TransferPolicy.Transfer_ID,
                        aRCHIVE_TransferPolicy_Original.Transfer_ID,
                        aRCHIVE_TransferPolicy.TransferPolicyType_ID,
                        aRCHIVE_TransferPolicy_Original.TransferPolicyType_ID,
                        aRCHIVE_TransferPolicy.TransferPolicyState_ID,
                        aRCHIVE_TransferPolicy_Original.TransferPolicyState_ID,
                        aRCHIVE_TransferPolicy.Registro_ID,
                        aRCHIVE_TransferPolicy_Original.Registro_ID,
                        aRCHIVE_TransferPolicy.UO_ID,
                        aRCHIVE_TransferPolicy_Original.UO_ID,
                        aRCHIVE_TransferPolicy.IncludiSottoalberoUO,
                        aRCHIVE_TransferPolicy_Original.IncludiSottoalberoUO,
                        aRCHIVE_TransferPolicy.Tipologia_ID,
                        aRCHIVE_TransferPolicy_Original.Tipologia_ID,
                        aRCHIVE_TransferPolicy.Titolario_ID,
                        aRCHIVE_TransferPolicy_Original.Titolario_ID,
                        aRCHIVE_TransferPolicy.ClasseTitolario,
                        aRCHIVE_TransferPolicy_Original.ClasseTitolario,
                        aRCHIVE_TransferPolicy.IncludiSottoalberoClasseTit,
                        aRCHIVE_TransferPolicy_Original.IncludiSottoalberoClasseTit,
                        aRCHIVE_TransferPolicy.AnnoCreazioneDa,
                        aRCHIVE_TransferPolicy_Original.AnnoCreazioneDa,
                        aRCHIVE_TransferPolicy.AnnoCreazioneA,
                        aRCHIVE_TransferPolicy_Original.AnnoCreazioneA,
                        aRCHIVE_TransferPolicy.AnnoProtocollazioneDa,
                        aRCHIVE_TransferPolicy_Original.AnnoProtocollazioneDa,
                        aRCHIVE_TransferPolicy.AnnoProtocollazioneA,
                        aRCHIVE_TransferPolicy_Original.AnnoProtocollazioneA,
                        aRCHIVE_TransferPolicy.AnnoChiusuraDa,
                        aRCHIVE_TransferPolicy_Original.AnnoChiusuraDa,
                        aRCHIVE_TransferPolicy.AnnoChiusuraA,
                        aRCHIVE_TransferPolicy_Original.AnnoChiusuraA,
                        aRCHIVE_TransferPolicy_Original.System_ID);
                return UpdateSucceded;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: UpdateARCHIVE_TransferPolicy", e);
                return false;
            }
        }
        /// <summary>
        /// Persists a new instance of ARCHIVE_TransferPolicy to the database.
        /// </summary>
        /// <param name="aRCHIVE_TransferPolicy">The instance of aRCHIVE_TransferPolicy to persist. </param>
        public static int InsertARCHIVE_TransferPolicy(ARCHIVE_TransferPolicy aRCHIVE_TransferPolicy)
        {
            try
            {
                int system_ID = 0;
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                depositoDB.InsertARCHIVE_TransferPolicy(aRCHIVE_TransferPolicy.Description, aRCHIVE_TransferPolicy.Enabled, aRCHIVE_TransferPolicy.Transfer_ID,
                aRCHIVE_TransferPolicy.TransferPolicyType_ID, aRCHIVE_TransferPolicy.TransferPolicyState_ID, aRCHIVE_TransferPolicy.Registro_ID, aRCHIVE_TransferPolicy.UO_ID, aRCHIVE_TransferPolicy.IncludiSottoalberoUO,
                aRCHIVE_TransferPolicy.Tipologia_ID, aRCHIVE_TransferPolicy.Titolario_ID, aRCHIVE_TransferPolicy.ClasseTitolario,
                aRCHIVE_TransferPolicy.IncludiSottoalberoClasseTit, aRCHIVE_TransferPolicy.AnnoCreazioneDa, aRCHIVE_TransferPolicy.AnnoCreazioneA,
                aRCHIVE_TransferPolicy.AnnoProtocollazioneDa, aRCHIVE_TransferPolicy.AnnoProtocollazioneA, aRCHIVE_TransferPolicy.AnnoChiusuraDa,
                aRCHIVE_TransferPolicy.AnnoChiusuraA, ref system_ID);

                return system_ID;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: DeleteARCHIVE_Transfer", e);
                return 0;
            }
        }

        //// <summary>
        /// Persists a new instance, or update an exist istance of ARCHIVE_TransferPolicy to the database, with all profileType associated.
        /// </summary>
        /// <param name="aRCHIVE_TransferPolicy">The instance of aRCHIVE_TransferPolicy to persist. </param>
        /// <param name="isA">true if type of policy is in arrivo</param>
        /// <param name="isP">true if type of policy is in partenza</param>
        /// <param name="isI">true if type of policy is interno</param>
        /// <param name="isNonProt">true if type of policy is non protcollato</param>
        /// <param name="isStRegProt">true if type of policy is stampa registro protocollo</param>
        /// <param name="isStRep">true if type of policy is stampa repertorio</param>
        /// 
        /// <returns name="system_id">return the system_id of ARCHIVETransferPolicy</returns>
        public static int Insert_UpdateARCHIVE_TransferPolicy(ARCHIVE_TransferPolicy aRCHIVE_TransferPolicy, bool isA, bool isP,
                                                        bool isI, bool isNonProt, bool isStRegProt, bool isStRep)
        {
            try
            {
                int system_ID;
                if (aRCHIVE_TransferPolicy.System_ID != 0)
                    system_ID = aRCHIVE_TransferPolicy.System_ID;
                else
                    system_ID = 0;

                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                depositoDB.Insert_UpdateARCHIVE_TransferPolicy(aRCHIVE_TransferPolicy.Description, aRCHIVE_TransferPolicy.Enabled, aRCHIVE_TransferPolicy.Transfer_ID,
                aRCHIVE_TransferPolicy.TransferPolicyType_ID, aRCHIVE_TransferPolicy.TransferPolicyState_ID, aRCHIVE_TransferPolicy.Registro_ID, aRCHIVE_TransferPolicy.UO_ID, aRCHIVE_TransferPolicy.IncludiSottoalberoUO,
                aRCHIVE_TransferPolicy.Tipologia_ID, aRCHIVE_TransferPolicy.Titolario_ID, aRCHIVE_TransferPolicy.ClasseTitolario,
                aRCHIVE_TransferPolicy.IncludiSottoalberoClasseTit, aRCHIVE_TransferPolicy.AnnoCreazioneDa, aRCHIVE_TransferPolicy.AnnoCreazioneA,
                aRCHIVE_TransferPolicy.AnnoProtocollazioneDa, aRCHIVE_TransferPolicy.AnnoProtocollazioneA, aRCHIVE_TransferPolicy.AnnoChiusuraDa,
                aRCHIVE_TransferPolicy.AnnoChiusuraA, isA, isP, isI, isNonProt, isStRegProt, isStRep, ref system_ID);

                return system_ID;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: Insert_UpdateARCHIVE_TransferPolicy", e);
                return 0;
            }
        }

        /// <summary>
        /// Updates all instance of ARCHIVE_TransferPolicy in Transfer.
        /// </summary>
        /// <param name="aRCHIVE_TransferPolicy">The instance of aRCHIVE_TransferPolicy to persist. </param>
        /// <param name="aRCHIVE_TransferPolicy">The original instance of aRCHIVE_TransferPolicy. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public static Boolean UpdateARCHIVE_TransferPolicyMassive(List<ARCHIVE_TransferPolicy> aRCHIVE_TransferPolicy)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();

                foreach (ARCHIVE_TransferPolicy _ereticPolicy in aRCHIVE_TransferPolicy)
                {
                    Boolean UpdateSucceded = depositoDB.UpdateARCHIVE_TransferPolicy(_ereticPolicy.Description, _ereticPolicy.Enabled,
                        _ereticPolicy.Transfer_ID, _ereticPolicy.TransferPolicyType_ID, _ereticPolicy.TransferPolicyState_ID, _ereticPolicy.Registro_ID,
                        _ereticPolicy.UO_ID, _ereticPolicy.IncludiSottoalberoUO, _ereticPolicy.Tipologia_ID,
                        _ereticPolicy.Titolario_ID, _ereticPolicy.ClasseTitolario, _ereticPolicy.IncludiSottoalberoClasseTit,
                        _ereticPolicy.AnnoCreazioneDa, _ereticPolicy.AnnoCreazioneA, _ereticPolicy.AnnoProtocollazioneDa,
                        _ereticPolicy.AnnoProtocollazioneA, _ereticPolicy.AnnoChiusuraDa, _ereticPolicy.AnnoChiusuraA, _ereticPolicy.System_ID);
                    if (!UpdateSucceded)
                        return false;
                }
                return true;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: UpdateARCHIVE_TransferPolicyMassive", e);
                return false;
            }
        }
        #endregion

        #region ARCHIVE_Profile_Transfer AND TransferPolicy

        /// <summary>
        /// Returns an instance of List<ARCHIVE_Profile_TransferPolicy>.
        /// </summary>
        /// <param name="TransferPolicy_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_Profile_TransferPolicy. </results>
        public static List<ARCHIVE_Profile_TransferPolicy> GetARCHIVE_Profile_TransferPolicyByTransferPolicy_ID(Int32 TransferPolicy_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_Profile_TransferPolicy> lista = depositoDB.GetARCHIVE_Profile_TransferPolicyByTransferPolicy_ID(TransferPolicy_ID);
                return lista;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetARCHIVE_Profile_TransferPolicyByTransferPolicy_ID", e);
                return null;
            }
        }

        /// <summary>
        /// Returns an instance of List<ARCHIVE_Profile_TransferPolicy>.
        /// </summary>
        /// <param name="TransferPolicyList">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_Profile_TransferPolicy. </results>
        public static List<ARCHIVE_Profile_TransferPolicy> GetARCHIVE_Profile_TransferPolicyByTransferPolicyList(string TransferPolicyList)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_Profile_TransferPolicy> lista = depositoDB.GetARCHIVE_Profile_TransferPolicyByTransferPolicyList(TransferPolicyList);
                return lista;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetARCHIVE_Profile_TransferPolicyByTransferPolicyList", e);
                return null;
            }
        }

        /// <summary>
        /// Update temp profile by profileList and transferID
        /// </summary>
        public static Boolean UpdateARCHIVE_Profile_TransferPolicyByProfileList(string ProfileList, Int32 TransferID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean result= depositoDB.UpdateARCHIVE_Profile_TransferPolicyByProfileList( ProfileList, TransferID);
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: UpdateARCHIVE_Profile_TransferPolicyByProfileList", e);
                return false;
            }
        }

        #endregion

        #region ARCHIVE_Transferpolicy_ProfileType

        /// <summary>
        /// Deletes an instance of ARCHIVE_Transferpolicy_ProfileType based on TransferPolicy_ID and ProfileType_ID.
        /// </summary>
        /// <param name="TransferPolicy_ID">The ID of  </param>
        /// <param name="ProfileType_ID">  </param>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public static Boolean DeleteARCHIVE_TransferPolicy_ProfileType(Int32 TransferPolicy_ID, Int32 ProfileType_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                bool deleteSuccess = depositoDB.DeleteARCHIVE_Transferpolicy_ProfileType(TransferPolicy_ID, ProfileType_ID);
                return deleteSuccess;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: DeleteARCHIVE_Transferpolicy_ProfileType", e);
                return false;
            }
        }

        /// <summary>
        /// Persists a new instance of ARCHIVE_TransferPolicy_ProfileType to the database.
        /// </summary>
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public static Boolean InsertARCHIVE_TransferPolicy_ProfileType(Int32 TransferPolicy_ID, Int32 ProfileType)
        {
            try
            {
                Boolean result;
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                result = depositoDB.InsertARCHIVE_TransferPolicy_ProfileType(TransferPolicy_ID, ProfileType);
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: InsertARCHIVE_TransferStateType", e);
                return false;
            }
        }

        /// <summary>
        /// return a list of ARCHIVE_Transferpolicy_ProfileType based on TransferPolicy_ID.
        /// </summary>
        /// <param name="TransferPolicy_ID">The ID of  </param>
        /// <param name="ProfileType_ID">  </param>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public static List<ARCHIVE_TransferPolicy_ProfileType> GetARCHIVE_TransferPolicy_ProfileTypeByTransferPolicyID(int TransferPolicy_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_TransferPolicy_ProfileType> TransferPolicy_ProfileType = depositoDB.GetARCHIVE_TransferPolicy_ProfileTypeByTransferPolicyID(TransferPolicy_ID);
                return TransferPolicy_ProfileType;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetARCHIVE_TransferPolicy_ProfileTypeBySystem_ID", e);
                return null;
            }

        }




        #endregion

        #region ARCHIVE_Transfer

        /// <summary>
        /// Returns an instance of List<ARCHIVE_Transfer>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_Transfer. </results>
        public static List<ARCHIVE_Transfer> GetARCHIVE_TransferBySystem_ID(Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_Transfer> Transfer = depositoDB.GetARCHIVE_TransferBySystem_ID(system_ID);
                return Transfer;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetARCHIVE_TransferBySystem_ID", e);
                return null;
            }
        }
        /// <summary>
        /// Returns all instances of ARCHIVE_Transfer.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_Transfer. </results>
        public static List<ARCHIVE_Transfer> GetAllARCHIVE_Transfer()
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_Transfer> Transfer = depositoDB.GetAllARCHIVE_Transfer();
                return Transfer;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetAllARCHIVE_Transfer", e);
                return null;
            }
        }
        /// <summary>
        /// Deletes an instance of ARCHIVE_Transfer based on System_ID.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public static Boolean DeleteARCHIVE_Transfer(Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean DeleteSucceded = depositoDB.DeleteARCHIVE_Transfer(system_ID);
                return DeleteSucceded;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: DeleteARCHIVE_Transfer", e);
                return false;
            }
        }
        /// <summary>
        /// Updates an instance of ARCHIVE_Transfer.
        /// </summary>
        /// <param name="note">This is not a required field. </param>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public static Boolean UpdateARCHIVE_Transfer(String description, String note, Int32 system_ID, Int32 id_Amministrazione)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean updateSuccess = depositoDB.UpdateARCHIVE_Transfer(description, note, system_ID, id_Amministrazione);
                return updateSuccess;
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
        public static Boolean UpdateARCHIVE_TransferOriginal(String description, String description_Original, String note,
                                                            String note_Original, Int32 system_ID, Int32 id_Amministrazione, Int32 id_Amministrazione_Original)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean updateSuccess = depositoDB.UpdateARCHIVE_Transfer(description, description_Original, note, note_Original, id_Amministrazione, id_Amministrazione_Original, system_ID);
                return updateSuccess;
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
        public static void InsertARCHIVE_Transfer(String description, String note, ref Int32 system_ID, Int32 iD_Amministrazione, Int32 transferStateType_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                depositoDB.InsertARCHIVE_Transfer(description, note, ref system_ID, iD_Amministrazione, transferStateType_ID);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: InsertARCHIVE_Transfer", e);
            }
        }

        #endregion

        #region ARCHIVE_TransferState

        /// <summary>
        /// Returns all instances of ARCHIVE_TransferState.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferState. </results>
        public static List<ARCHIVE_TransferState> GetAllARCHIVE_TransferState()
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_TransferState> archive_states = depositoDB.GetAllARCHIVE_TransferState();
                return archive_states;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetAll", e);
                return null;
            }
        }
        /// <summary>
        /// Returns an instance of List<ARCHIVE_TransferState>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferState. </results>
        public static List<ARCHIVE_TransferState> GetARCHIVE_TransferStateBySystem_ID(Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_TransferState> archive_state = depositoDB.GetARCHIVE_TransferStateBySystem_ID(system_ID);
                return archive_state;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetARCHIVE_TransferStateBySystem_ID", e);
                return null;
            }
        }
        /// <summary>
        /// Relates ARCHIVE_Transfer to ARCHIVE_TransferState.
        /// Returns a collection of ARCHIVE_TransferState based on the following criteria: Transfer_ID.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferState. </results>
        public static List<ARCHIVE_TransferState> GetARCHIVE_TransferStateByTransfer_ID(Int32 transfer_ID)
        {

            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_TransferState> archive_state = depositoDB.GetARCHIVE_TransferStateByTransfer_ID(transfer_ID);
                return archive_state;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetARCHIVE_TransferStateByTransfer_ID", e);
                return null;
            }
        }
        /// <summary>
        /// Returns a collection of ARCHIVE_TransferState based on the following criteria: TransferStateType_ID.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferState. </results>
        public static List<ARCHIVE_TransferState> GetARCHIVE_TransferStateByTransferStateType_ID(Int32 transferStateType_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_TransferState> archive_state = depositoDB.GetARCHIVE_TransferStateByTransferStateType_ID(transferStateType_ID);
                return archive_state;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetARCHIVE_TransferStateByTransferStateType_ID", e);
                return null;
            }
        }
        /// <summary>
        /// Deletes an instance of ARCHIVE_TransferState based on System_ID.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public static Boolean DeleteARCHIVE_TransferState(Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                bool deleteSuccess = depositoDB.DeleteARCHIVE_TransferState(system_ID);
                return deleteSuccess;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: DeleteARCHIVE_TransferState", e);
                return false;
            }
        }
        /// <summary>
        /// Deletes an instance of ARCHIVE_TransferState based on Transfer_ID.
        /// </summary>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public static Boolean DeleteARCHIVE_TransferStateByTransfer_ID(Int32 transfer_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                bool deleteSuccess = depositoDB.DeleteARCHIVE_TransferStateByTransfer_ID(transfer_ID);
                return deleteSuccess;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: DeleteARCHIVE_TransferStateByTransfer_ID", e);
                return false;
            }
        }
        /// <summary>
        /// Updates an instance of ARCHIVE_TransferState.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public static Boolean UpdateARCHIVE_TransferState(Int32 transfer_ID, Int32 transferStateType_ID, Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean updateSuccess = depositoDB.UpdateARCHIVE_TransferState(transfer_ID, transferStateType_ID, system_ID);
                return updateSuccess;
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
        public static Boolean UpdateARCHIVE_TransferState(Int32 transfer_ID, Int32 transfer_ID_Original, Int32 transferStateType_ID, Int32 transferStateType_ID_Original, Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean updateSuccess = depositoDB.UpdateARCHIVE_TransferState(transfer_ID, transferStateType_ID, system_ID);
                return updateSuccess;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: UpdateARCHIVE_TransferState", e);
                return false;
            }
        }
        /// <summary>
        /// Persists a new instance of ARCHIVE_TransferState to the database.
        /// </summary>
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public static void InsertARCHIVE_TransferState(Int32 transfer_ID, Int32 transferStateType_ID, ref Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                depositoDB.InsertARCHIVE_TransferState(transfer_ID, transferStateType_ID, ref system_ID);
            }
            catch (Exception e)
            {
                logger.Debug("Errore - metodo: InsertARCHIVE_TransferStateType", e);
            }
        }

        #endregion

        #region ARCHIVE_TransferStateType

        /// <summary>
        /// Returns all instances of ARCHIVE_TransferStateType.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferStateType. </results>
        public static List<ARCHIVE_TransferStateType> GetAllARCHIVE_TransferStateType()
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_TransferStateType> archive_states_types = depositoDB.GetAllARCHIVE_TransferStateType();
                return archive_states_types;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetAllARCHIVE_TransferStateType", e);
                return null;
            }
        }
        /// <summary>
        /// Returns an instance of List<ARCHIVE_TransferStateType>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferStateType. </results>
        public static List<ARCHIVE_TransferStateType> GetARCHIVE_TransferStateTypeBySystem_ID(Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_TransferStateType> archive_states_types = depositoDB.GetARCHIVE_TransferStateTypeBySystem_ID(system_ID);
                return archive_states_types;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetARCHIVE_TransferStateTypeBySystem_ID", e);
                return null;
            }
        }
        /// <summary>
        /// Deletes an instance of ARCHIVE_TransferStateType based on System_ID.
        /// </summary>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public static Boolean DeleteARCHIVE_TransferStateType(Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean delSuccess = depositoDB.DeleteARCHIVE_TransferStateType(system_ID);
                return delSuccess;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: DeleteARCHIVE_TransferStateType", e);
                return false;
            }
        }
        /// <summary>
        /// Deletes all instances of ARCHIVE_TransferStateType.
        /// </summary>
        /// <results>The number of items deleted. </results>
        public static Int32 DeleteAllARCHIVE_TransferStateType()
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                int rowEffected = depositoDB.DeleteAllARCHIVE_TransferStateType();
                return rowEffected;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: DeleteAllARCHIVE_TransferStateType", e);
                return -1;
            }
        }
        /// <summary>
        /// Updates an instance of ARCHIVE_TransferStateType.
        /// </summary>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public static Boolean UpdateARCHIVE_TransferStateType(String name, Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean updateSuccess = depositoDB.UpdateARCHIVE_TransferStateType(name, system_ID);
                return updateSuccess;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: UpdateARCHIVE_TransferStateType", e);
                return false;
            }
        }
        /// <summary>
        /// Updates an instance of ARCHIVE_TransferStateType if the original data has not changed.
        /// </summary>
        /// <param name="name_Original">This field is used for optimistic concurrency management. It should contain the original value of 'name_Original'. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public static Boolean UpdateARCHIVE_TransferStateType(String name, String name_Original, Int32 system_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean updateSuccess = depositoDB.UpdateARCHIVE_TransferStateType(name, name_Original, system_ID);
                return updateSuccess;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: UpdateARCHIVE_TransferStateType", e);
                return false;
            }
        }
        /// <summary>
        /// Persists a new instance of ARCHIVE_TransferStateType to the database.
        /// </summary>
        public static Boolean InsertARCHIVE_TransferStateType(Int32 system_ID, String name)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                Boolean updateSuccess = depositoDB.InsertARCHIVE_TransferStateType(system_ID, name);
                return updateSuccess;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: InsertARCHIVE_TransferStateType", e);
                return false;
            }
        }

        #endregion

        #region ARCHIVE_View_Policy

        /// <summary>
        /// Returns an instance of List<ARCHIVE_View_Documents_Policy>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_View_Documents_Policy. </results>
        public static List<ARCHIVE_View_Policy> GetAllARCHIVE_View_Policy(Int32 transferID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_View_Policy> archive_view_policy = depositoDB.GetAllARCHIVE_View_Policy(transferID);
                return archive_view_policy;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetAllARCHIVE_View_Policy", e);
                return null;
            }
        }

        #endregion

        #region ARCHIVE_View_Documents_Policy

        /// <summary>
        /// Returns an instance of List<ARCHIVE_View_Documents_Policy>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_View_Documents_Policy. </results>
        public static List<ARCHIVE_View_Documents_Policy> GetAllARCHIVE_View_Documents_Policy(String transferPolicyList)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_View_Documents_Policy> archive_view_doc_DG = depositoDB.GetAllARCHIVE_View_Documents_Policy(transferPolicyList);
                return archive_view_doc_DG;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetAllARCHIVE_View_Documents_Policy", e);
                return null;
            }
        }

        #endregion

        #region ARCHIVE_View_Projects_Policy

        /// <summary>
        /// Returns an instance of List<ARCHIVE_View_Projects_Policy>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_View_Projects_Policy. </results>
        public static List<ARCHIVE_View_Projects_Policy> GetAllARCHIVE_View_Projects_Policy(String transferPolicyList)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_View_Projects_Policy> archive_view_doc_DG = depositoDB.GetAllARCHIVE_View_Projects_Policy(transferPolicyList);
                return archive_view_doc_DG;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetAllARCHIVE_View_Projects_Policy", e);
                return null;
            }
        }

        #endregion

        #region ARCHIVE_View_Result_Transfer

        /// <summary>
        /// Returns an instance of List<ARCHIVE_Result_Transfer>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_Result_Transfer. </results>
        public static List<ARCHIVE_Result_Transfer> GetAllARCHIVE_Result_Transfer(String transferList)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_Result_Transfer> archive_view_Transfer_in_DG = depositoDB.GetAllARCHIVE_Result_Transfer(transferList);
                return archive_view_Transfer_in_DG;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetAllARCHIVE_View_Projects_Policy", e);
                return null;
            }
        }

        #endregion

        #region ARCHIVE_View_FascDocReport/ARCHIVE_View_DocReport

        /// <summary>
        /// Returns an instance of List<ARCHIVE_View_FascReport>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_JOB_TransferPolicy. </results>
        public static List<ARCHIVE_View_FascReport> GetARCHIVE_View_FascReportByTransfer_ID(Int32 transfer_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_View_FascReport> list = depositoDB.GetARCHIVE_View_ARCHIVE_FascReportByTransferID(transfer_ID);
                return list;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetARCHIVE_View_FascReportByTransferPolicy_ID", e);
                return null;
            }
        }

        /// <summary>
        /// Returns an instance of List<ARCHIVE_View_FascReport>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_JOB_TransferPolicy. </results>
        public static List<ARCHIVE_View_DocReport> GetARCHIVE_View_DocReportByTransfer_ID(Int32 transfer_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_View_DocReport> list = depositoDB.GetARCHIVE_View_ARCHIVE_DocReportByTransferID(transfer_ID);
                return list;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetARCHIVE_View_DocReportByTransferPolicy_ID", e);
                return null;
            }
        }





        #endregion

        #region ARCHIVE_View_FascDocReportDisposal/ARCHIVE_View_DocReportDisposal

        /// <summary>
        /// Returns an instance of List<ARCHIVE_View_FascReportDisposal>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_View_FascReportDisposal. </results>
        public static List<ARCHIVE_View_FascReportDisposal> GetARCHIVE_View_FascReportByDisposal_ID(Int32 Disposal_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_View_FascReportDisposal> list = depositoDB.GetARCHIVE_View_ARCHIVE_FascReportByDisposalID(Disposal_ID);
                return list;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetARCHIVE_View_FascReportByDisposal_ID", e);
                return null;
            }
        }

        /// <summary>
        /// Returns an instance of List<ARCHIVE_View_FascReportDisposal>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_View_FascReportDisposal. </results>
        public static List<ARCHIVE_View_DocReportDisposal> GetARCHIVE_View_DocReportByDisposal_ID(Int32 Disposal_ID)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Deposito depositoDB = new DocsPaDB.Query_DocsPAWS.Deposito();
                List<DocsPaVO.Deposito.ARCHIVE_View_DocReportDisposal> list = depositoDB.GetARCHIVE_View_ARCHIVE_DocReportByDisposalID(Disposal_ID);
                return list;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: GetARCHIVE_View_DocReportByDisposal_ID", e);
                return null;
            }
        }





        #endregion

    }
}
