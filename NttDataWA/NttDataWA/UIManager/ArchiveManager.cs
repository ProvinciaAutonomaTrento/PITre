using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.Utils;
using System.Collections;
using NttDataWA.DocsPaWR;


namespace NttDataWA.UIManager
{
    public static class ArchiveManager
    {
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        public static Dictionary<int, string> _dictionaryDisposalState = new Dictionary<int, string>
        {
	            {1, "IN DEFINIZIONE"},
                {2, "RICERCA COMPLETATA"},
	            {3, "PROPOSTO"},
	            {4, "APPROVATO"},
	            {5, "IN ESECUZIONE"},
	            {6, "EFFETTUATO"},
	            {7, "IN ERRORE"}
              
	           
        };

        public static Dictionary<int, string> _dictionaryTransferState = new Dictionary<int, string>
        {
	            {1, "IN DEFINIZIONE"},
                {2, "ANALISI COMPLETATA"},
	            {3, "PROPOSTO"},
	            {4, "APPROVATO"},
	            {5, "IN ESECUZIONE"},
	            {6, "EFFETTUATO"},
	            {7, "IN ERRORE"}
              
	           
        };
        public static Dictionary<int, string> _dictionaryTransferPolicyState = new Dictionary<int, string>
        {
            {1,"RICERCA NON AVVIATA"},
            {2,"RICERCA IN CORSO"},
            {3,"RICERCA COMPLETATA"},
            {4,"ANALISI IN CORSO"},
            {5,"ANALISI COMPLETATA"}
        };

        public static Dictionary<int, string> _dictionaryTransferPolicyType = new Dictionary<int, string>
        {
            {1,"DOCUMENTI"},
            {2,"FASCICOLI"}

        };

        public static Dictionary<int, string> _dictionaryProfileType = new Dictionary<int, string>
        {
            {1,"ARRIVO"},
            {2,"PARTENZA"},
            {3,"INTERNO"},
            {4,"NON PROTOCOLLATO"},
            {5,"STAMPA REGISTRO PROTOCOLLATO"},
            {6,"STAMPA REPERTORIO"}

        };

        #region UpdateDisposalTEMP

        public static Boolean Update_ARCHIVE_TempProjectDisposal(Int32 Disposal_ID, string listProjectsID)
        {
            try
            {
                bool result = docsPaWS.Update_ARCHIVE_TempProjectDisposal(Disposal_ID, listProjectsID);
                return result;
            }

            catch (System.Exception ex)
            {

                return false;
            }
        }

        public static Boolean Update_ARCHIVE_TempProfileDisposal(Int32 Disposal_ID, string listProfilesID)
        {
            try
            {
                bool result = docsPaWS.Update_ARCHIVE_TempProfileDisposal(Disposal_ID, listProfilesID);
                return result;
            }

            catch (System.Exception ex)
            {

                return false;
            }
        }

        #endregion


        #region Archive_Disposal_Job

        /// <summary>
        /// Persists a new instance of ARCHIVE_JOB_Transfer to the database.
        /// </summary>
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public static void InsertARCHIVE_JOB_Disposal(Int32 Disposal_ID, Int32 jobType_ID, ref Int32 system_ID)
        {
            try
            {
                docsPaWS.InsertARCHIVE_JOB_Disposal(Disposal_ID, jobType_ID, ref  system_ID);
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Returns an instance of List<ARCHIVE_JOB_Disposal>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_JOB_Disposal. </results>
        public static List<DocsPaWR.ARCHIVE_JOB_Disposal> GetARCHIVE_JOB_DisposalByDisposal_ID(Int32 disposal_ID)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_JOB_Disposal> result = docsPaWS.GetARCHIVE_JOB_DisposalByDisposal_ID(disposal_ID).Cast<DocsPaWR.ARCHIVE_JOB_Disposal>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {
                return null;
                //throw new Exception(ex.Message);
            }
        }
        #endregion 

        #region Archive_Disposal_view

        /// <summary>
        /// Returns all instances of ARCHIVE_TransferPolicy view.
        /// Per questioni di tempo devo usare questo oggetto anche per gli scarti.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_View_Policy. </results>
        public static List<DocsPaWR.ARCHIVE_View_Documents_Policy> GetAllARCHIVE_View_Documents_Disposal(Int32 disposal_id)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_View_Documents_Policy> result = docsPaWS.GetAllARCHIVE_View_Documents_Disposal(disposal_id).Cast<DocsPaWR.ARCHIVE_View_Documents_Policy>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }
        /// <summary>
        /// Returns all instances of ARCHIVE_TransferPolicy view.
        /// Per questioni di tempo devo usare questo oggetto anche per gli scarti.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_View_Policy. </results>
        public static List<DocsPaWR.ARCHIVE_View_Projects_Policy> GetAllARCHIVE_View_Projects_Disposal(Int32 disposal_id)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_View_Projects_Policy> result = docsPaWS.GetAllARCHIVE_View_Projects_Disposal(disposal_id).Cast<DocsPaWR.ARCHIVE_View_Projects_Policy>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }
      

        #endregion

        #region Archive_Disposal
        public static List<ARCHIVE_DisposalForSearch> GetAllARCHIVE_DisposalFilterForSearch(String st_indefinizione, String st_analisicompletata, String st_proposto,
                                                                               String st_approvato, String st_inesecuzione, String st_effettuato, String st_inerrore, Int32 finger)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_DisposalForSearch> result = docsPaWS.GetAllARCHIVE_DisposalFilterForSearch(st_indefinizione, st_analisicompletata, st_proposto,
                                                                                 st_approvato, st_inesecuzione, st_effettuato, st_inerrore, finger).Cast<DocsPaWR.ARCHIVE_DisposalForSearch>().ToList();

                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }

        /// <summary>
        /// Returns an instance of List<OnlyBS.Data.DOCSADM.ARCHIVE_Disposal>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_Disposal. </results>
        public static List<DocsPaWR.ARCHIVE_Disposal> GetARCHIVE_DisposalBySystem_ID(Int32 system_ID)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_Disposal> result = docsPaWS.GetARCHIVE_DisposalBySystem_ID(system_ID).Cast<DocsPaWR.ARCHIVE_Disposal>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }

        /// <summary>
        /// Returns all instances of ARCHIVE_Disposal.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_Disposal. </results>
        public static List<DocsPaWR.ARCHIVE_Disposal> GetAllARCHIVE_Disposal()
        {
            try
            {
                List<DocsPaWR.ARCHIVE_Disposal> result = docsPaWS.GetAllARCHIVE_Disposal().Cast<DocsPaWR.ARCHIVE_Disposal>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

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
                bool result = docsPaWS.DeleteARCHIVE_Disposal(system_ID);
                return result;
            }

            catch (System.Exception ex)
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
        public static Boolean UpdateARCHIVE_Disposal(String description, String note, Int32 system_ID, Int32 iD_Amministrazione)
        {
            try
            {
                bool result = docsPaWS.UpdateARCHIVE_Disposal(description, note, system_ID, iD_Amministrazione);
                return result;
            }
            catch (System.Exception ex)
            {
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
        public static Boolean UpdateARCHIVE_DisposalOriginal(String description, String description_Original, String note, String note_Original, Int32 iD_Amministrazione, Int32 iD_Amministrazione_Original, Int32 system_ID)
        {
            try
            {
                bool result = docsPaWS.UpdateARCHIVE_DisposalOriginal(description, description_Original, note, note_Original, system_ID, iD_Amministrazione, iD_Amministrazione_Original);
                return result;
            }

            catch (System.Exception ex)
            {

                return false;
            }
        }

        /// <summary>
        /// Persists a new instance of ARCHIVE_Disposal to the database.
        /// </summary>
        /// <param name="note">This is not a required field. </param>
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public static void InsertARCHIVE_Disposal(String description, String note, ref Int32 system_ID, Int32 iD_Amministrazione, Int32 transferStateType_ID)
        {
            try
            {
                docsPaWS.InsertARCHIVE_Disposal(description, note, ref  system_ID, iD_Amministrazione, transferStateType_ID);
            }
            catch (System.Exception ex)
            {

            }
        }

        #endregion

        #region Archive_Disposal_state

        /// <summary>
        /// Returns all instances of ARCHIVE_DisposalState.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_DisposalState. </results>
        public static List<DocsPaWR.ARCHIVE_DisposalState> GetAllDisposalState()
        {
            try
            {
                List<DocsPaWR.ARCHIVE_DisposalState> result = docsPaWS.GetAllARCHIVE_DisposalState().Cast<DocsPaWR.ARCHIVE_DisposalState>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }

        /// <summary>
        /// Returns an instance of List<DocsPaVO.Deposito.DOCSADM.ARCHIVE_DisposalState>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_DisposalState. </results>
        public static List<DocsPaWR.ARCHIVE_DisposalState> GetARCHIVE_DisposalStateBySystem_ID(Int32 System_ID)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_DisposalState> result = docsPaWS.GetARCHIVE_DisposalStateBySystem_ID(System_ID).Cast<DocsPaWR.ARCHIVE_DisposalState>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }

        /// <summary>
        /// Relates ARCHIVE_Disposal to ARCHIVE_DisposalState.
        /// Returns a collection of ARCHIVE_DisposalState based on the following criteria: Disposal_ID.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_DisposalState. </results>
        public static List<DocsPaWR.ARCHIVE_DisposalState> GetARCHIVE_DisposalStateByDisposal_ID(Int32 Disposal_ID)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_DisposalState> result = docsPaWS.GetARCHIVE_DisposalStateByDisposal_ID(Disposal_ID).Cast<DocsPaWR.ARCHIVE_DisposalState>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }

        /// <summary>
        /// Returns a collection of ARCHIVE_DisposalState based on the following criteria: DisposalStateType_ID.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_DisposalState. </results>
        public static List<DocsPaWR.ARCHIVE_DisposalState> GetARCHIVE_DisposalStateByDisposalStateType_ID(Int32 transferStateType_ID)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_DisposalState> result = docsPaWS.GetARCHIVE_DisposalStateByDisposalStateType_ID(transferStateType_ID).Cast<DocsPaWR.ARCHIVE_DisposalState>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {
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
                return docsPaWS.DeleteARCHIVE_DisposalState(system_ID);
            }
            catch (System.Exception ex)
            {

                return false;
            }
        }

        /// <summary>
        /// Deletes an instance of ARCHIVE_DisposalState based on Disposal_ID.
        /// </summary>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public static Boolean DeleteARCHIVE_DisposalStateByDisposal_ID(Int32 transfer_ID)
        {
            try
            {
                return docsPaWS.DeleteARCHIVE_DisposalStateByDisposal_ID(transfer_ID);
            }
            catch (System.Exception ex)
            {

                return false;
            }
        }

        /// <summary>
        /// Updates an instance of ARCHIVE_DisposalState.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public static Boolean UpdateARCHIVE_DisposalState(Int32 transfer_ID, Int32 transferStateType_ID, Int32 system_ID)
        {
            try
            {
                return docsPaWS.UpdateARCHIVE_DisposalState(transfer_ID, transferStateType_ID, system_ID);
            }
            catch (System.Exception ex)
            {

                return false;
            }
        }

        /// <summary>
        /// Updates an instance of ARCHIVE_DisposalState if the original data has not changed.
        /// </summary>
        /// <param name="transfer_ID_Original">This field is used for optimistic concurrency management. It should contain the original value of 'transfer_ID_Original'. </param>
        /// <param name="transferStateType_ID_Original">This field is used for optimistic concurrency management. It should contain the original value of 'transferStateType_ID_Original'. </param>
        /// <param name="dateTime_Original">This field is used for optimistic concurrency management. It should contain the original value of 'dateTime_Original'. </param>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public static Boolean UpdateARCHIVE_DisposalStateOriginal(Int32 transfer_ID, Int32 transfer_ID_Original, Int32 transferStateType_ID, Int32 transferStateType_ID_Original, Int32 system_ID)
        {
            try
            {
                return docsPaWS.UpdateARCHIVE_DisposalStateOriginal(transfer_ID, transfer_ID_Original, transferStateType_ID, transferStateType_ID_Original, system_ID);
            }
            catch (System.Exception ex)
            {

                return false;
            }
        }

        /// <summary>
        /// Persists a new instance of ARCHIVE_DisposalState to the database.
        /// </summary>
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public static void InsertARCHIVE_DisposalState(Int32 transfer_ID, Int32 transferStateType_ID, ref Int32 system_ID)
        {
            try
            {
                docsPaWS.InsertARCHIVE_DisposalState(transfer_ID, transferStateType_ID, ref  system_ID);
            }
            catch (System.Exception ex)
            {

            }
        }
        #endregion

        #region Archive_Disposal_state_types

        /// <summary>
        /// Returns all instances of ARCHIVE_DisposalStateType.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_DisposalStateType. </results>
        public static List<DocsPaWR.ARCHIVE_DisposalStateType> GetAllARCHIVE_DisposalStateType()
        {
            try
            {
                List<DocsPaWR.ARCHIVE_DisposalStateType> result = docsPaWS.GetAllARCHIVE_DisposalStateType().Cast<DocsPaWR.ARCHIVE_DisposalStateType>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }

        /// <summary>
        /// Returns an instance of List<Only2table.Data.DOCSADM.ARCHIVE_DisposalStateType>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_DisposalStateType. </results>
        public static List<DocsPaWR.ARCHIVE_DisposalStateType> GetARCHIVE_DisposalStateTypeBySystem_ID(Int32 system_ID)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_DisposalStateType> result = docsPaWS.GetARCHIVE_DisposalStateTypeBySystem_ID(system_ID).Cast<DocsPaWR.ARCHIVE_DisposalStateType>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

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
                return docsPaWS.DeleteARCHIVE_DisposalStateType(system_ID);
            }
            catch (System.Exception ex)
            {

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
                return docsPaWS.DeleteAllARCHIVE_DisposalStateType();
            }
            catch (System.Exception ex)
            {

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
                return docsPaWS.UpdateARCHIVE_DisposalStateType(name, system_ID);
            }
            catch (System.Exception ex)
            {

                return false;
            }
        }

        /// <summary>
        /// Updates an instance of ARCHIVE_DisposalStateType if the original data has not changed.
        /// </summary>
        /// <param name="name_Original">This field is used for optimistic concurrency management. It should contain the original value of 'name_Original'. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public static Boolean UpdateARCHIVE_DisposalStateTypeOriginal(String name, String name_Original, Int32 system_ID)
        {
            try
            {
                return docsPaWS.UpdateARCHIVE_DisposalStateTypeOriginal(name, name_Original, system_ID);
            }
            catch (System.Exception ex)
            {

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
                return docsPaWS.InsertARCHIVE_DisposalStateType(system_ID, name);
            }
            catch (System.Exception ex)
            {

                return false;
            }
        }

        public static object GetDataSourceVuotoPerResultSearchDisposal()
        {
            List<DocsPaWR.ARCHIVE_DisposalForSearch> _lstNullDatasource = new List<DocsPaWR.ARCHIVE_DisposalForSearch>();
            DocsPaWR.ARCHIVE_DisposalForSearch _ResultDisposalDummy = new DocsPaWR.ARCHIVE_DisposalForSearch();
            _ResultDisposalDummy.DateTime = DateTime.Now;
            _ResultDisposalDummy.Description = "";
            _ResultDisposalDummy.Numero_doc_scartati = 0;
            _ResultDisposalDummy.System_id = 0;
            _ResultDisposalDummy.Stato = "";
            _lstNullDatasource.Add(_ResultDisposalDummy);
            return _lstNullDatasource;
        }

        #endregion

        #region ARCHIVE_AUTH

        /// <summary>
        /// Returns an instance of List<ARCHIVE_AUTH_Authorization>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_AUTH_Authorization. </results>
        public static List<ARCHIVE_AUTH_Authorization> GetARCHIVE_AutorizationBySystem_ID(Int32 system_ID)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_AUTH_Authorization> result = docsPaWS.GetARCHIVE_AutorizationBySystem_ID(system_ID).Cast<DocsPaWR.ARCHIVE_AUTH_Authorization>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

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
                List<DocsPaWR.ARCHIVE_AUTH_Authorization> result = docsPaWS.GetALLARCHIVE_Autorizations().Cast<DocsPaWR.ARCHIVE_AUTH_Authorization>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

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
                bool result = docsPaWS.DeleteARCHIVE_Autorizations_BySystem_ID(system_ID);
                return result;
            }

            catch (System.Exception ex)
            {

                return false;
            }
        }

        /// <summary>
        /// Persists a new instance of ARCHIVE_Transfer to the database.
        /// </summary>
        /// <param name="note">This is not a required field. </param>
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public static void InsertARCHIVE_Authorization(Int32 People_ID, DateTime StartDate, DateTime EndDate, String note, String profileList,
                                                        String projectList, ref Int32 system_ID)
        {
            try
            {
                docsPaWS.InsertARCHIVE_Authorization(People_ID, StartDate.ToString(), EndDate.ToString(), note, profileList, projectList, ref system_ID);
            }
            catch (System.Exception ex)
            {

            }
        }

        /// <summary>
        /// Updates an instance of ARCHIVE_AUTH_Authorization.
        /// </summary>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public static Boolean UpdateARCHIVE_Authorization(Int32 People_ID, DateTime StartDate, DateTime EndDate, String note, String profileList,
                                                        String projectList, ref Int32 system_ID)
        {
            try
            {
                bool result = docsPaWS.UpdateARCHIVE_Authorization(People_ID, StartDate.ToString(), EndDate.ToString(), note, profileList, projectList, ref system_ID);
                return result;
            }

            catch (System.Exception ex)
            {

                return false;
            }
        }

        #endregion

        #region ARCHIVE_AUTH_OBJECT

        /// <summary>
        /// Returns an instance of List<ARCHIVE_AUTH_AuthorizedObject>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_AUTH_AuthorizedObject. </results>
        public static List<ARCHIVE_AUTH_AuthorizedObject> GetARCHIVE_AutorizedObjectBySystem_ID(Int32 system_ID)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_AUTH_AuthorizedObject> result = docsPaWS.GetARCHIVE_AutorizedObjectBySystem_ID(system_ID).Cast<DocsPaWR.ARCHIVE_AUTH_AuthorizedObject>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }

        /// <summary>
        /// Returns all instance of List<ARCHIVE_AUTH_AuthorizedObject>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_AUTH_AuthorizedObject. </results>
        public static List<ARCHIVE_AUTH_AuthorizedObject> GetALLARCHIVE_AutorizedObject()
        {
            try
            {
                List<DocsPaWR.ARCHIVE_AUTH_AuthorizedObject> result = docsPaWS.GetALLARCHIVE_AutorizedObject().Cast<DocsPaWR.ARCHIVE_AUTH_AuthorizedObject>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }

        /// <summary>
        /// Deletes an instance of ARCHIVE_AUTH_Authorization based on System_ID.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public static Boolean DeleteARCHIVE_AutorizedObject_BySystem_ID(Int32 system_ID)
        {
            try
            {
                bool result = docsPaWS.DeleteARCHIVE_AutorizedObject_BySystem_ID(system_ID);
                return result;
            }

            catch (System.Exception ex)
            {

                return false;
            }
        }

        /// <summary>
        /// Persists a new instance of InsertARCHIVE_AutorizedObject to the database.
        /// </summary>
        /// <param name="note">This is not a required field. </param>
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public static void InsertARCHIVE_AutorizedObject(Int32 Authorization_ID, Int32 Project_ID, Int32 profile_ID, ref Int32 system_ID)
        {
            try
            {
                docsPaWS.InsertARCHIVE_AutorizedObject(Authorization_ID, Project_ID, profile_ID, ref  system_ID);
            }
            catch (System.Exception ex)
            {

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
                bool result = docsPaWS.UpdateARCHIVE_AutorizedObject(Authorization_ID, Project_ID, profile_ID, ref  system_ID);
                return result;
            }

            catch (System.Exception ex)
            {

                return false;
            }
        }

        #endregion

        #region ARCHIVE_LOG

        /// <summary>
        /// Returns an instance of List<ARCHIVE_JOB_Transfer>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_JOB_Transfer. </results>
        public static List<DocsPaWR.ARCHIVE_LOG_TransferAndPolicy> GetARCHIVE_LOG_TransferAndPolicy(String ListaVersamentoIDANDPolicyID)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_LOG_TransferAndPolicy> result = docsPaWS.GetARCHIVE_LOG_TransferAndPolicy(ListaVersamentoIDANDPolicyID).Cast<DocsPaWR.ARCHIVE_LOG_TransferAndPolicy>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {
                return null;
                //throw new Exception(ex.Message);
            }
        }
        #endregion

        #region ARCHIVE_JOB_Transfer

        /// <summary>
        /// Persists a new instance of ARCHIVE_JOB_Transfer to the database.
        /// </summary>
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public static void InsertARCHIVE_JOB_Transfer(Int32 transfer_ID, Int32 jobType_ID, ref Int32 system_ID)
        {
            try
            {
                docsPaWS.InsertARCHIVE_JOB_Transfer(transfer_ID, jobType_ID, ref  system_ID);
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Returns an instance of List<ARCHIVE_JOB_Transfer>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_JOB_Transfer. </results>
        public static List<DocsPaWR.ARCHIVE_JOB_Transfer> GetARCHIVE_JOB_TransferByTransfer_ID(Int32 transfer_ID)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_JOB_Transfer> result = docsPaWS.GetARCHIVE_JOB_TransferByTransfer_ID(transfer_ID).Cast<DocsPaWR.ARCHIVE_JOB_Transfer>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {
                return null;
                //throw new Exception(ex.Message);
            }
        }

        #endregion

        #region ARCHIVE_JOB_TransferPolicy
        /// <summary>
        /// Returns an instance of List<ARCHIVE_JOB_TransferPolicy>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_JOB_TransferPolicy, related on a transfer. </results>
        public static List<DocsPaWR.ARCHIVE_JOB_TransferPolicy> GetARCHIVE_JOB_TransferPolicyByTransfer_ID(Int32 transfer_ID)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_JOB_TransferPolicy> result = docsPaWS.GetARCHIVE_JOB_TransferPolicyByTransfer_ID(transfer_ID).Cast<DocsPaWR.ARCHIVE_JOB_TransferPolicy>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {
                return null;
                //throw new Exception(ex.Message);
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
                docsPaWS.InsertARCHIVE_JOB_Transfer(transferPolicy_ID, jobType_ID, ref  system_ID);
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Returns an instance of List<ARCHIVE_JOB_TransferPolicy>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_JOB_TransferPolicy. </results>
        public static List<DocsPaWR.ARCHIVE_JOB_TransferPolicy> GetARCHIVE_JOB_TransferPolicyByTransferPolicy_ID(Int32 transferPolicy_ID)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_JOB_TransferPolicy> result = docsPaWS.GetARCHIVE_JOB_TransferByTransfer_ID(transferPolicy_ID).Cast<DocsPaWR.ARCHIVE_JOB_TransferPolicy>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {
                return null;
                //throw new Exception(ex.Message);
            }
        }

        #endregion

        #region utility for Achive_transfer_policy

        /// <summary>
        /// Utility che crea una stringa contenente una stringa con gli id policy selezionati separati da virgole
        /// a partire da una lista di id di interi. Utile per richiamare la start serch
        /// </summary>
        /// <param name="list">lista di interi Int32 contente gli id policy selezionati</param>
        /// <returns></returns>
        public static string GetSQLinStringFromListIdPolicy(List<Int32> list)
        {
            int brk = 0;
            string sqlIn = string.Empty;

            foreach (Int32 id in list)
            {
                if (brk == 0)
                {
                    sqlIn = "" + id.ToString();
                    brk++;
                }
                else
                    sqlIn += "," + id.ToString();
            }
            sqlIn += "";

            return sqlIn;
        }

        /// <summary>
        /// avvia la ricerca di un'insieme di policy 
        /// </summary>
        /// <param name="ListSystemID"> </param>
        /// <results> </results>
        public static Boolean StartSearchForTransferPolicyListByPolicyId(string ListSystemID)
        {
            try
            {
                Boolean result = docsPaWS.StartSearchForTransferPolicyList(ListSystemID);
                return result;
            }

            catch (System.Exception ex)
            {

                return false;
            }
        }

        /// <summary>
        /// avvia l'analisi di un'insieme di policy 
        /// </summary>
        /// <param name="ListSystemID"> </param>
        /// <results> </results>
        public static Boolean StartAnalysisForTransferPolicyListByPolicyId(string ListSystemID)
        {
            try
            {
                // Boolean result = docsPaWS.start(ListSystemID);
                return true;
            }

            catch (System.Exception ex)
            {

                return false;
            }
        }

        public static ArrayList GetListaUOForTransferPolicy(string prefixText, string contextKey)
        {
            ArrayList listaTemp;
            DocsPaWR.ParametriRicercaRubrica qco = new DocsPaWR.ParametriRicercaRubrica();
            DocsPaWR.InfoUtente infoUtente = new DocsPaWR.InfoUtente();
            qco.caller = new DocsPaWR.RubricaCallerIdentity();
            qco.parent = "";
            char[] delimiterChars = { '-' };
            string[] splitData = contextKey.Split(delimiterChars);
            qco.caller.IdRuolo = splitData[0];
            qco.caller.IdRegistro = splitData[1];
            qco.descrizione = prefixText;
            string callType = splitData[3];
            infoUtente.idAmministrazione = splitData[2];

            //case "CALLTYPE_OWNER_AUTHOR":
            qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_OWNER_AUTHOR;
            //qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_TRASM_ALL;
            qco.tipoIE = DocsPaWR.AddressbookTipoUtente.INTERNO;

            qco.doRuoli = false;
            qco.doUo = true;
            qco.doUtenti = false;
            qco.doListe = false;
            qco.doRF = false;
            qco.doRubricaComune = false;

            listaTemp = new ArrayList(docsPaWS.GetElementiRubrica(qco, infoUtente));

            return listaTemp;
        }



        #endregion

        #region Achive_transfer_policy
        /// <summary>
        /// Deletes an instance of ARCHIVE_TransferPolicy based on System_ID.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>o
        public static Boolean DeleteARCHIVE_TransferPolicyList(string listIdPolicy)
        {
            try
            {
                Boolean result = docsPaWS.DeleteARCHIVE_TransferPolicyList(listIdPolicy);
                return result;
            }

            catch (System.Exception ex)
            {

                return false;
            }
        }

        /// <summary>
        /// avvia l'analisi (sincrona) di un'insieme di policy 
        /// </summary>
        /// <param name="ListSystemID"> </param>
        /// <returns>ritorna una lista tipizzata in Int32</returns>
        public static List<Int32> StartAsyncAnalysisForTransferPolicyListByPolicyId(string ListSystemID)
        {
            try
            {
                List<Int32> result = docsPaWS.StartAsyncAnalisysForTransferPolicyList(ListSystemID).ToList();

                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }

        /// <summary>
        /// avvia l'ricerca (sincrona) di un'insieme di policy 
        /// </summary>
        /// <param name="ListSystemID"> </param>
        /// <returns>ritorna una lista tipizzata in Int32</returns>
        public static List<Int32> StartAsyncSearchForTransferPolicyListByPolicyId(string ListSystemID)
        {
            try
            {
                List<Int32> result = docsPaWS.StartAsyncSearchForTransferPolicyList(ListSystemID).ToList();

                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }
        /// <summary>
        /// Returns a collection of ARCHIVE_TransferPolicy based on the following criteria: Transfer_ID.
        /// </summary>
        /// <param name="transfer_ID">This is not a required field. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferPolicy. </results>
        public static List<DocsPaWR.ARCHIVE_TransferPolicy> GetARCHIVE_TransferPolicyByTransfer_ID(Int32? transfer_ID)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_TransferPolicy> result = docsPaWS.GetARCHIVE_TransferPolicyByTransfer_ID(transfer_ID).Cast<DocsPaWR.ARCHIVE_TransferPolicy>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }
        /// <summary>
        /// Returns an instance of List<ARCHIVE_TransferPolicy>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferPolicy. </results>
        public static List<DocsPaWR.ARCHIVE_TransferPolicy> GetARCHIVE_TransferPolicyBySystem_ID(Int32 system_ID)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_TransferPolicy> result = docsPaWS.GetARCHIVE_TransferBySystem_ID(system_ID).Cast<DocsPaWR.ARCHIVE_TransferPolicy>().ToList();
                return result;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// Returns all instances of ARCHIVE_TransferPolicy view.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_View_Policy. </results>
        public static List<DocsPaWR.ARCHIVE_View_Policy> GetAllARCHIVE_View_Policy(Int32 IdTransfer)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_View_Policy> result = docsPaWS.GetAllARCHIVE_View_Policy(IdTransfer).Cast<DocsPaWR.ARCHIVE_View_Policy>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }
        /// <summary>
        /// Returns all instances of ARCHIVE_TransferPolicy view.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_View_Policy. </results>
        public static List<DocsPaWR.ARCHIVE_View_Documents_Policy> GetAllARCHIVE_View_Documents_Policy(String ConcatenatetransferPolicy)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_View_Documents_Policy> result = docsPaWS.GetAllARCHIVE_View_Documents_Policy(ConcatenatetransferPolicy).Cast<DocsPaWR.ARCHIVE_View_Documents_Policy>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }
        
        /// <summary>
        /// Returns all instances of ARCHIVE_TransferPolicy view.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_View_Policy. </results>
        public static List<DocsPaWR.ARCHIVE_View_Projects_Policy> GetAllARCHIVE_View_Projects_Policy(String ConcatenatetransferPolicy)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_View_Projects_Policy> result = docsPaWS.GetAllARCHIVE_View_Projects_Policy(ConcatenatetransferPolicy).Cast<DocsPaWR.ARCHIVE_View_Projects_Policy>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }
        /// <summary>
        /// Returns all instances of ARCHIVE_TransferPolicy view.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_View_Policy. </results>
        public static List<DocsPaWR.ARCHIVE_View_Documents_Policy> TransferPolicyViewDocumentsPolicyInContext(String transferPolicyConcatenateInString)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_View_Documents_Policy> result = docsPaWS.GetAllARCHIVE_View_Documents_Policy(transferPolicyConcatenateInString).Cast<DocsPaWR.ARCHIVE_View_Documents_Policy>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }

        }
        /// <summary>
        /// Returns all instances of ARCHIVE_TransferPolicy.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferPolicy. </results>
        public static List<DocsPaWR.ARCHIVE_TransferPolicy> GetAllARCHIVE_TransferPolicy()
        {
            try
            {
                List<DocsPaWR.ARCHIVE_TransferPolicy> result = docsPaWS.GetAllARCHIVE_TransferPolicy().Cast<DocsPaWR.ARCHIVE_TransferPolicy>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }
        /// <summary>
        /// Relates ARCHIVE_TransferPolicyType to ARCHIVE_TransferPolicy.
        /// Returns a collection of ARCHIVE_TransferPolicy based on the following criteria: TransferPolicyType_ID.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferPolicy. </results>
        public static List<DocsPaWR.ARCHIVE_TransferPolicy> GetARCHIVE_TransferPolicyByTransferPolicyType_ID(Int32 transferPolicyType_ID)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_TransferPolicy> result = docsPaWS.GetARCHIVE_TransferPolicyByTransferPolicyType_ID(transferPolicyType_ID).Cast<DocsPaWR.ARCHIVE_TransferPolicy>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }
        /// <summary>
        /// Deletes an instance of ARCHIVE_TransferPolicy based on System_ID.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>o
        public static Boolean DeleteARCHIVE_TransferPolicy(Int32 system_ID)
        {
            try
            {
                Boolean result = docsPaWS.DeleteARCHIVE_TransferPolicy(system_ID);
                return result;
            }

            catch (System.Exception ex)
            {

                return false;
            }
        }
        /// <summary>
        /// Deletes all instances of ARCHIVE_TransferPolicy.
        /// </summary>
        /// <results>The number of items deleted. </results>
        public static Int32 DeleteAllARCHIVE_TransferPolicy()
        {
            try
            {
                Int32 result = docsPaWS.DeleteAllARCHIVE_TransferPolicy();
                return result;
            }

            catch (System.Exception ex)
            {

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
                Boolean result = docsPaWS.DeleteARCHIVE_TransferPolicyByTransferPolicyType_ID(transferPolicyType_ID);
                return result;
            }

            catch (System.Exception ex)
            {

                return false;
            }
        }
        /// <summary>
        /// Updates an instance of ARCHIVE_TransferPolicy.
        /// </summary>
        /// <param name="aRCHIVE_TransferPolicy">The instance of aRCHIVE_TransferPolicy to persist. </param>
        /// <param name="aRCHIVE_TransferPolicy">The original instance of aRCHIVE_TransferPolicy. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public static Boolean UpdateARCHIVE_TransferPolicy(DocsPaWR.ARCHIVE_TransferPolicy aRCHIVE_TransferPolicy)
        {
            try
            {
                Boolean result = docsPaWS.UpdateARCHIVE_TransferPolicy(aRCHIVE_TransferPolicy);
                return result;
            }

            catch (System.Exception ex)
            {

                return false;
            }
        }

        /// <summary>
        /// Updates an instance of ARCHIVE_TransferPolicy.
        /// </summary>
        /// <param name="aRCHIVE_TransferPolicy">The instance of aRCHIVE_TransferPolicy to persist. </param>
        /// <param name="aRCHIVE_TransferPolicy">The original instance of aRCHIVE_TransferPolicy. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public static Boolean UpdateARCHIVE_TransferPolicy(DocsPaWR.ARCHIVE_TransferPolicy aRCHIVE_TransferPolicy, bool isA, bool isP,
                                                        bool isI, bool isNonProt, bool isStRegProt, bool isStRep)
        {
            try
            {
                int system_id = 0;
                system_id = docsPaWS.Insert_UpdateARCHIVE_TransferPolicyAndProfileType(aRCHIVE_TransferPolicy, isA, isP, isI, isNonProt, isStRegProt, isStRep);
                return system_id>0;
            }

            catch (System.Exception ex)
            {

                return false;
            }
        }

        /// <summary>
        /// Updates an instance of ARCHIVE_TransferPolicy.
        /// </summary>
        /// <param name="aRCHIVE_TransferPolicy">The instance of aRCHIVE_TransferPolicy to persist. </param>
        /// <param name="aRCHIVE_TransferPolicy">The original instance of aRCHIVE_TransferPolicy. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public static Boolean UpdateARCHIVE_TransferPolicyMassive(List<DocsPaWR.ARCHIVE_TransferPolicy> aRCHIVE_TransferPolicy)
        {
            try
            {
                Boolean result = docsPaWS.UpdateARCHIVE_TransferPolicyMassive(aRCHIVE_TransferPolicy.ToArray());
                return true;
            }

            catch (System.Exception ex)
            {

                return false;
            }
        }


        /// <summary>
        /// Updates an instance of ARCHIVE_TransferPolicy.
        /// </summary>
        /// <param name="aRCHIVE_TransferPolicy">The instance of aRCHIVE_TransferPolicy to persist. </param>
        /// <param name="aRCHIVE_TransferPolicy">The original instance of aRCHIVE_TransferPolicy. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public static Boolean UpdateARCHIVE_TransferPolicyOriginal(List<DocsPaWR.ARCHIVE_TransferPolicy> aRCHIVE_TransferPolicy, List<DocsPaWR.ARCHIVE_TransferPolicy> aRCHIVE_TransferPolicy_Original)
        {
            try
            {
                Boolean result = false;
                for (int i = 0; i < aRCHIVE_TransferPolicy.Count(); i++)
                {
                    result = docsPaWS.UpdateARCHIVE_TransferPolicyOriginal(aRCHIVE_TransferPolicy[i], aRCHIVE_TransferPolicy_Original[i]);
                }
                return result;
            }

            catch (System.Exception ex)
            {

                return false;
            }
        }
        /// <summary>
        /// Persists a new instance of ARCHIVE_TransferPolicy to the database.
        /// </summary>
        /// <param name="aRCHIVE_TransferPolicy">The instance of aRCHIVE_TransferPolicy to persist. </param>
        public static int InsertARCHIVE_TransferPolicy(DocsPaWR.ARCHIVE_TransferPolicy aRCHIVE_TransferPolicy)
        {
            try
            {
                int system_id = 0;
                system_id = docsPaWS.InsertARCHIVE_TransferPolicy(aRCHIVE_TransferPolicy);
                return system_id;
            }

            catch (System.Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// Persists a new instance of ARCHIVE_TransferPolicy to the database, with the ProfileType associated.
        /// </summary>
        /// <param name="aRCHIVE_TransferPolicy">The instance of aRCHIVE_TransferPolicy to persist. </param>
        public static int InsertARCHIVE_TransferPolicy(DocsPaWR.ARCHIVE_TransferPolicy aRCHIVE_TransferPolicy, bool isA, bool isP,
                                                        bool isI, bool isNonProt, bool isStRegProt, bool isStRep)
        {
            try
            {
                int system_id = 0;
                system_id = docsPaWS.Insert_UpdateARCHIVE_TransferPolicyAndProfileType(aRCHIVE_TransferPolicy, isA, isP, isI, isNonProt, isStRegProt, isStRep);
                return system_id;
            }

            catch (System.Exception ex)
            {
                return 0;
            }
        }

        #endregion

        #region ARCHIVE_Profile_TransferPolicy
        /// <summary>
        /// Returns a collection of ARCHIVE_TransferPolicy based on the following criteria: Transfer_ID.
        /// </summary>
        /// <param name="transferPolicy_ID">This is not a required field. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferPolicy. </results>
        public static List<DocsPaWR.ARCHIVE_Profile_TransferPolicy> GetARCHIVE_Profile_TransferPolicyByTransferPolicy_ID(Int32 transferPolicy_ID)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_Profile_TransferPolicy> result = docsPaWS.GetARCHIVE_Profile_TransferPolicyByTransferPolicy_ID(transferPolicy_ID).Cast<DocsPaWR.ARCHIVE_Profile_TransferPolicy>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }

        /// <summary>
        /// Returns a collection of ARCHIVE_TransferPolicy based on the following criteria: Transfer_ID.
        /// </summary>
        /// <param name="transferPolicyList">This is not a required field. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferPolicy. </results>
        public static List<DocsPaWR.ARCHIVE_Profile_TransferPolicy> GetARCHIVE_Profile_TransferPolicyByTransferPolicyList(string transferPolicyList)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_Profile_TransferPolicy> result = docsPaWS.GetARCHIVE_Profile_TransferPolicyByTransferPolicyList(transferPolicyList).Cast<DocsPaWR.ARCHIVE_Profile_TransferPolicy>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }

        public static Boolean UpdateARCHIVE_Profile_TransferPolicyByProfileList(string listProfilesID, Int32 TransferID)
        {
            try
            {
                bool result = docsPaWS.UpdateARCHIVE_Profile_TransferPolicyByProfileList(listProfilesID, TransferID);
                return result;
            }

            catch (System.Exception ex)
            {

                return false;
            }
        }

        #endregion

        #region Archive_Transfer

        /// <summary>
        /// Returns an instance of List<OnlyBS.Data.DOCSADM.ARCHIVE_Transfer>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_Transfer. </results>
        public static List<DocsPaWR.ARCHIVE_Transfer> GetARCHIVE_TransferBySystem_ID(Int32 system_ID)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_Transfer> result = docsPaWS.GetARCHIVE_TransferBySystem_ID(system_ID).Cast<DocsPaWR.ARCHIVE_Transfer>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }

        /// <summary>
        /// Returns all instances of ARCHIVE_Transfer.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_Transfer. </results>
        public static List<DocsPaWR.ARCHIVE_Transfer> GetAllARCHIVE_Transfer()
        {
            try
            {
                List<DocsPaWR.ARCHIVE_Transfer> result = docsPaWS.GetAllARCHIVE_Transfer().Cast<DocsPaWR.ARCHIVE_Transfer>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

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
                bool result = docsPaWS.DeleteARCHIVE_Transfer(system_ID);
                return result;
            }

            catch (System.Exception ex)
            {

                return false;
            }
        }

        /// <summary>
        /// Updates an instance of ARCHIVE_Transfer.
        /// </summary>
        /// <param name="note">This is not a required field. </param>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public static Boolean UpdateARCHIVE_Transfer(String description, String note, Int32 system_ID, Int32 iD_Amministrazione)
        {
            try
            {
                bool result = docsPaWS.UpdateARCHIVE_Transfer(description, note, system_ID, iD_Amministrazione);
                return result;
            }

            catch (System.Exception ex)
            {

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
        public static Boolean UpdateARCHIVE_TransferOriginal(String description, String description_Original, String note, String note_Original, Int32 iD_Amministrazione, Int32 iD_Amministrazione_Original, Int32 system_ID)
        {
            try
            {
                bool result = docsPaWS.UpdateARCHIVE_TransferOriginal(description, description_Original, note, note_Original, system_ID, iD_Amministrazione, iD_Amministrazione_Original);
                return result;
            }

            catch (System.Exception ex)
            {

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
                docsPaWS.InsertARCHIVE_Transfer(description, note, ref  system_ID, iD_Amministrazione, transferStateType_ID);
            }
            catch (System.Exception ex)
            {

            }
        }

        #endregion

        #region Archive_Transfer_state

        /// <summary>
        /// Returns all instances of ARCHIVE_TransferState.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferState. </results>
        public static List<DocsPaWR.ARCHIVE_TransferState> GetAllTransferState()
        {
            try
            {
                List<DocsPaWR.ARCHIVE_TransferState> result = docsPaWS.GetAllARCHIVE_TransferState().Cast<DocsPaWR.ARCHIVE_TransferState>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }

        /// <summary>
        /// Returns an instance of List<DocsPaVO.Deposito.DOCSADM.ARCHIVE_TransferState>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferState. </results>
        public static List<DocsPaWR.ARCHIVE_TransferState> GetARCHIVE_TransferStateBySystem_ID(Int32 System_ID)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_TransferState> result = docsPaWS.GetARCHIVE_TransferStateBySystem_ID(System_ID).Cast<DocsPaWR.ARCHIVE_TransferState>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }

        /// <summary>
        /// Relates ARCHIVE_Transfer to ARCHIVE_TransferState.
        /// Returns a collection of ARCHIVE_TransferState based on the following criteria: Transfer_ID.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferState. </results>
        public static List<DocsPaWR.ARCHIVE_TransferState> GetARCHIVE_TransferStateByTransfer_ID(Int32 Transfer_ID)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_TransferState> result = docsPaWS.GetARCHIVE_TransferStateByTransfer_ID(Transfer_ID).Cast<DocsPaWR.ARCHIVE_TransferState>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }

        /// <summary>
        /// Returns a collection of ARCHIVE_TransferState based on the following criteria: TransferStateType_ID.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferState. </results>
        public static List<DocsPaWR.ARCHIVE_TransferState> GetARCHIVE_TransferStateByTransferStateType_ID(Int32 transferStateType_ID)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_TransferState> result = docsPaWS.GetARCHIVE_TransferStateByTransferStateType_ID(transferStateType_ID).Cast<DocsPaWR.ARCHIVE_TransferState>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {
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
                return docsPaWS.DeleteARCHIVE_TransferState(system_ID);
            }
            catch (System.Exception ex)
            {

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
                return docsPaWS.DeleteARCHIVE_TransferStateByTransfer_ID(transfer_ID);
            }
            catch (System.Exception ex)
            {

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
                return docsPaWS.UpdateARCHIVE_TransferState(transfer_ID, transferStateType_ID, system_ID);
            }
            catch (System.Exception ex)
            {

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
        public static Boolean UpdateARCHIVE_TransferStateOriginal(Int32 transfer_ID, Int32 transfer_ID_Original, Int32 transferStateType_ID, Int32 transferStateType_ID_Original, Int32 system_ID)
        {
            try
            {
                return docsPaWS.UpdateARCHIVE_TransferStateOriginal(transfer_ID, transfer_ID_Original, transferStateType_ID, transferStateType_ID_Original, system_ID);
            }
            catch (System.Exception ex)
            {

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
                docsPaWS.InsertARCHIVE_TransferState(transfer_ID, transferStateType_ID, ref  system_ID);
            }
            catch (System.Exception ex)
            {

            }
        }
        #endregion

        #region ARCHIVE_TransferPolicy_ProfileType

        /// <summary>
        /// Deletes an instance of ARCHIVE_TransferPolicy_ProfileType based on TransferPolicy_ID and ProfileType.
        /// </summary>
        /// <param name="TransferPolicy_ID"></param>
        /// <param name="TransferPolicy_ID"></param>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public static Boolean DeleteARCHIVE_TransferPolicy_ProfileType(Int32 TransferPolicy_ID, Int32 ProfileType)
        {
            try
            {
                return docsPaWS.DeleteARCHIVE_TransferPolicy_ProfileType(TransferPolicy_ID, ProfileType);
            }
            catch (System.Exception ex)
            {

                return false;
            }
        }

        /// <summary>
        /// Insert an instance of ARCHIVE_TransferPolicy_ProfileType
        /// </summary>
        /// <param name="TransferPolicy_ID"></param>
        /// <param name="TransferPolicy_ID"></param>
        public static void InsertARCHIVE_TransferPolicy_ProfileType(Int32 TransferPolicy_ID, Int32 ProfileType)
        {
            try
            {
                docsPaWS.InsertARCHIVE_TransferPolicy_ProfileType(TransferPolicy_ID, ProfileType);
            }
            catch (System.Exception ex)
            {

            }
        }

        /// <summary>
        /// Returns an instance of List<ARCHIVE_TransferPolicy_ProfileType>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferPolicy_ProfileType. </results>
        public static List<DocsPaWR.ARCHIVE_TransferPolicy_ProfileType> GetARCHIVE_TransferPolicy_ProfileTypeByTransferPolicy_ID(Int32 system_ID)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_TransferPolicy_ProfileType> result = docsPaWS.GetARCHIVE_TransferPolicy_ProfileTypeByTransferPolicy_ID(system_ID).Cast<DocsPaWR.ARCHIVE_TransferPolicy_ProfileType>().ToList();
                return result;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }



        #endregion

        #region Archive_Transfer_state_types

        /// <summary>
        /// Returns all instances of ARCHIVE_TransferStateType.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferStateType. </results>
        public static List<DocsPaWR.ARCHIVE_TransferStateType> GetAllARCHIVE_TransferStateType()
        {
            try
            {
                List<DocsPaWR.ARCHIVE_TransferStateType> result = docsPaWS.GetAllARCHIVE_TransferStateType().Cast<DocsPaWR.ARCHIVE_TransferStateType>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }

        /// <summary>
        /// Returns an instance of List<Only2table.Data.DOCSADM.ARCHIVE_TransferStateType>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferStateType. </results>
        public static List<DocsPaWR.ARCHIVE_TransferStateType> GetARCHIVE_TransferStateTypeBySystem_ID(Int32 system_ID)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_TransferStateType> result = docsPaWS.GetARCHIVE_TransferStateTypeBySystem_ID(system_ID).Cast<DocsPaWR.ARCHIVE_TransferStateType>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

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
                return docsPaWS.DeleteARCHIVE_TransferStateType(system_ID);
            }
            catch (System.Exception ex)
            {

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
                return docsPaWS.DeleteAllARCHIVE_TransferStateType();
            }
            catch (System.Exception ex)
            {

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
                return docsPaWS.UpdateARCHIVE_TransferStateType(name, system_ID);
            }
            catch (System.Exception ex)
            {

                return false;
            }
        }

        /// <summary>
        /// Updates an instance of ARCHIVE_TransferStateType if the original data has not changed.
        /// </summary>
        /// <param name="name_Original">This field is used for optimistic concurrency management. It should contain the original value of 'name_Original'. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public static Boolean UpdateARCHIVE_TransferStateTypeOriginal(String name, String name_Original, Int32 system_ID)
        {
            try
            {
                return docsPaWS.UpdateARCHIVE_TransferStateTypeOriginal(name, name_Original, system_ID);
            }
            catch (System.Exception ex)
            {

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
                return docsPaWS.InsertARCHIVE_TransferStateType(system_ID, name);
            }
            catch (System.Exception ex)
            {

                return false;
            }
        }

        public static object GetDataSourceVuotoPerResultSearchTransfer()
        {
            List<DocsPaWR.ARCHIVE_TransferForSearch> _lstNullDatasource = new List<DocsPaWR.ARCHIVE_TransferForSearch>();
            DocsPaWR.ARCHIVE_TransferForSearch _ResultTransferDummy = new DocsPaWR.ARCHIVE_TransferForSearch();
            _ResultTransferDummy.DateTime = DateTime.Now;
            _ResultTransferDummy.Description = "";
            _ResultTransferDummy.Numero_doc_copie = 0;
            _ResultTransferDummy.Numero_doc_effettivi = 0;
            _ResultTransferDummy.System_id = 0;
            _ResultTransferDummy.Stato = "";
            _lstNullDatasource.Add(_ResultTransferDummy);
            return _lstNullDatasource;
        }

        public static List<DocsPaWR.ARCHIVE_View_Documents_Policy> GetDataSourceVuotoPerGrigliaDocumentiPolicy()
        {
            List<DocsPaWR.ARCHIVE_View_Documents_Policy> _lstNullDatasource = new List<DocsPaWR.ARCHIVE_View_Documents_Policy>();
            DocsPaWR.ARCHIVE_View_Documents_Policy _policyDocumentDummy = new DocsPaWR.ARCHIVE_View_Documents_Policy();
            _policyDocumentDummy.Registro = "";
            _policyDocumentDummy.Titolario = "";
            _policyDocumentDummy.Classetitolario = "";
            _policyDocumentDummy.Tipologia = "";
            //_policyDocumentDummy.AnnoCreazione = 0;
            _policyDocumentDummy.Totale = 0;
            _lstNullDatasource.Add(_policyDocumentDummy);
            return _lstNullDatasource;
        }

        public static List<DocsPaWR.ARCHIVE_View_Projects_Policy> GetDataSourceVuotoPerGrigliaFascicoliPolicy()
        {
            List<DocsPaWR.ARCHIVE_View_Projects_Policy> _lstNullDatasource = new List<DocsPaWR.ARCHIVE_View_Projects_Policy>();
            DocsPaWR.ARCHIVE_View_Projects_Policy _policyProjectsDummy = new DocsPaWR.ARCHIVE_View_Projects_Policy();
            _policyProjectsDummy.Registro = "";
            _policyProjectsDummy.Titolario = "";
            _policyProjectsDummy.Classetitolario = "";
            _policyProjectsDummy.Tipologia = "";
            //_policyProjectsDummy.AnnoChiusura = 0;
            _policyProjectsDummy.Totale = 0;
            _lstNullDatasource.Add(_policyProjectsDummy);
            return _lstNullDatasource;
        }

        public static List<DocsPaWR.ARCHIVE_View_Policy> GetDataSourceVuotoPerGrigliaPolicy()
        {
            List<DocsPaWR.ARCHIVE_View_Policy> _lstNullDatasource = new List<DocsPaWR.ARCHIVE_View_Policy>();
            DocsPaWR.ARCHIVE_View_Policy _policyDummy = new DocsPaWR.ARCHIVE_View_Policy();
            _policyDummy.Id_policy = 0;
            _policyDummy.Descrizione = "";
            _policyDummy.Num_documenti_copiati = 0;
            _policyDummy.Num_documenti_trasferiti = 0;
            _policyDummy.Totale_documenti = 0;
            _policyDummy.Totale_fascicoli = 0;
            _lstNullDatasource.Add(_policyDummy);
            return _lstNullDatasource;
        }


        #endregion

        #region Archive_Result_Transfer

        public static List<DocsPaWR.ARCHIVE_Result_Transfer> GetResultTransferInGrid(String TransferList)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_Result_Transfer> result = docsPaWS.GetAllARCHIVE_Result_Transfer(TransferList).Cast<DocsPaWR.ARCHIVE_Result_Transfer>().ToList();
                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }

        #endregion

        #region Utilities X Search

        public static Boolean StartSearchForTransferPolicy(Int32 system_ID)
        {
            try
            {
                return docsPaWS.StartSearchForTransferPolicy(system_ID);
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public static Boolean StartAnalysisForTransfer(Int32 system_ID)
        {
            try
            {
                return docsPaWS.StartAnalysisForTransfer(system_ID);
            }
            catch (System.Exception ex)
            {
                return false;
            }

        }

        public static List<ARCHIVE_TransferForSearch> GetAllARCHIVE_TransferFilterForSearch(String st_indefinizione, String st_analisicompletata, String st_proposto,
                                                                                String st_approvato, String st_inesecuzione, String st_effettuato, String st_inerrore, Int32 finger)
        {
            try
            {
                List<DocsPaWR.ARCHIVE_TransferForSearch> result = docsPaWS.GetAllARCHIVE_TransferFilterForSearch(st_indefinizione, st_analisicompletata, st_proposto,
                                                                                 st_approvato, st_inesecuzione, st_effettuato, st_inerrore, finger).Cast<DocsPaWR.ARCHIVE_TransferForSearch>().ToList();

                return result;
            }

            catch (System.Exception ex)
            {

                return null;
            }
        }

        #endregion

        #region ARCHIVE_View_FascDocReport/Disposal

        /// <summary>
        /// Returns an instance of List<ARCHIVE_View_FascReport>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_JOB_TransferPolicy. </results>
        public static List<ARCHIVE_View_FascReportDisposal> GetARCHIVE_View_FascReportByDisposal_ID(Int32 Disposal_ID)
        {
            try
            {
                List<ARCHIVE_View_FascReportDisposal> list = docsPaWS.GetARCHIVE_View_FascReportByDisposal_ID(Disposal_ID).Cast<ARCHIVE_View_FascReportDisposal>().ToList();
                return list;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        /// <summary>
        /// Returns an instance of List<ARCHIVE_View_DocReport>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_View_DocReport. </results>
        public static List<ARCHIVE_View_DocReportDisposal> GetARCHIVE_View_DocReportByDisposal_ID(Int32 Disposal_ID)
        {
            try
            {
                List<ARCHIVE_View_DocReportDisposal> list = docsPaWS.GetARCHIVE_View_DocReportByDisposal_ID(Disposal_ID).Cast<ARCHIVE_View_DocReportDisposal>().ToList();
                return list;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Returns an instance of List<ARCHIVE_View_FascReport>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_JOB_TransferPolicy. </results>
        public static List<ARCHIVE_View_FascReport> GetARCHIVE_View_FascReportByTransfer_ID(Int32 transfer_ID)
        {
            try
            {
                List<ARCHIVE_View_FascReport> list = docsPaWS.GetARCHIVE_View_FascReportByTransfer_ID(transfer_ID).Cast<ARCHIVE_View_FascReport>().ToList();
                return list;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        /// <summary>
        /// Returns an instance of List<ARCHIVE_View_DocReport>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_View_DocReport. </results>
        public static List<ARCHIVE_View_DocReport> GetARCHIVE_View_DocReportByTransfer_ID(Int32 transfer_ID)
        {
            try
            {
                List<ARCHIVE_View_DocReport> list = docsPaWS.GetARCHIVE_View_DocReportByTransfer_ID(transfer_ID).Cast<ARCHIVE_View_DocReport>().ToList();
                return list;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        #endregion

    }
}
