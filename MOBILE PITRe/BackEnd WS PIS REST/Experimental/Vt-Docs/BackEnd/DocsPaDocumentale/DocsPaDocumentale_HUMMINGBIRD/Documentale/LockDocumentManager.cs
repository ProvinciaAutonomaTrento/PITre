using System;
using DocsPaDB;
using log4net;

namespace DocsPaDocumentale_HUMMINGBIRD.Documentale
{
	/// <summary>
	/// Classe per l'interazione con le api di hummingbird
	/// per la gestione del blocco/sblocco dei documenti
	/// </summary>
	internal sealed class LockDocumentManager
	{
        private static ILog logger = LogManager.GetLogger(typeof(LockDocumentManager));

		private LockDocumentManager()
		{
		}

		/// <summary>
		/// Impostazione del blocco sul documento nel documentale hummingbird
		/// </summary>
		/// <param name="documentNumber"></param>
		/// <param name="documentLocation"></param>
		/// <param name="idUser"></param>
		/// <param name="library"></param>
		/// <returns></returns>
		public static bool Lock(string documentNumber,string documentLocation,string idUser,string dst,string library)
		{
			bool retValue=false;

			try
			{
				PCDCLIENTLib.PCDDocObject obj = new PCDCLIENTLib.PCDDocObjectClass();

				//impostazione del DST
				obj.SetDST(dst);

				//impostiamo la libreria
				obj.SetProperty("%TARGET_LIBRARY",library);
				
				//impostiamo la form
				obj.SetObjectType("DEF_PROF");

				//specifichiamo il documento
				obj.SetProperty("%OBJECT_IDENTIFIER",documentNumber);

				obj.SetProperty("%CHECKIN_LOCATION",documentLocation);

				//impostiamo lo stato del documento a checkout
				//e creiamo una entry nella tabella checkout
				obj.SetProperty("%STATUS","%LOCK_FOR_CHECKOUT");

				//la modifica del documento è legata al particolare utente
				obj.SetProperty("%TYPIST",idUser);

				//COMMENTI__________________________________________________________________________________________________
				//PER I COMMENTI SCEGLIERE UNA DELLE DUE OPZIONI SEGUENTI:

				//impostiamo nei commenti l'utente e la data
				//obj.SetProperty("%CHECKOUT_COMMENTS",infoUtente.idPeople + Convert.ToString(DateTime.Now));

				//Recuperiamo la descrizione dell'utente
				//string user = getUserDesc(infoUtente.idPeople,co);

				//impostiamo i commenti a quanto ci viene passato dal front
				obj.SetProperty("%CHECKOUT_COMMENTS",string.Empty);
				//__________________________________________________________________________________________________________
				
				obj.Update();

				retValue=(obj.ErrNumber==0);

				if (!retValue)
					logger.Debug("Lock ERROR:" + obj.ErrDescription);
			}
			catch (Exception ex)
			{	
				logger.Debug("Lock ERROR:" + ex.Message);

				throw ex;
			}

			return retValue;
		}

		/// <summary>
		/// Rimozione del blocco sul documento nel documentale hummingbird
		/// </summary>
		/// <param name="documentNumber"></param>
		/// <param name="dst"></param>
		/// <param name="library"></param>
		/// <returns></returns>
		public static bool UnLock(string documentNumber,string dst,string library)
		{
			bool retValue=false;

			try
			{
				if (dst!=null && dst!=string.Empty && library!=null && library!=string.Empty)
				{
					PCDCLIENTLib.PCDDocObject obj = new PCDCLIENTLib.PCDDocObjectClass();

					//impostazione del DST
					obj.SetDST(dst);

					//impostiamo la libreria
					obj.SetProperty("%TARGET_LIBRARY",library);

					//impostiamo la form
					obj.SetObjectType("DEF_PROF");

					//specifichiamo il documento
					obj.SetProperty("%OBJECT_IDENTIFIER",documentNumber);

					obj.SetProperty("%STATUS","%UNLOCK");

					obj.SetProperty("%CHECKIN_DATE",DateTime.Now.ToString());

					obj.Update();

					retValue=(obj.ErrNumber==0);

					if (!retValue)
					{
						logger.Debug("UnLock ERROR:" + obj.ErrDescription);
					}
				}
				else
				{
					// Rimozione manuale del blocco sul documento, 
					// qualora non vengano forniti i parametri dst e library
					retValue=ManualUnlock(documentNumber);
				}
			}
			catch(Exception ex)
			{
				logger.Debug("UnLock ERROR:" + ex.Message);

				retValue=false;
			}

			return retValue;
		}

        /// <summary>
        /// Impostazione manuale del blocco sul documento nel documentale hummingbird.
        /// 1. Creazione del record dalla tabella "CHECKOUT"
        /// 2. Impostazione del campo "STATUS" a 3 nella tabella "PROFILE" relativa
        ///    al documento da sbloccare 
        /// </summary>
        /// <param name="documentNumber"></param>
        /// <param name="documentLocation"></param>
        /// <param name="idPeople"></param>
        /// <param name="dst"></param>
        /// <param name="library"></param>
        /// <returns></returns>
        public static bool ManualLock(string documentNumber, string versionLabel, string documentLocation, string idPeople, string dst, string library)
        {
            bool retValue = false;

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    dbProvider.BeginTransaction();

                    DateTime lockDate = DateTime.Now;

                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("HUMMY_I_MANUAL_CHECKOUT_DOCUMENT");
                    queryDef.setParam("colId", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    queryDef.setParam("systemId", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
                    queryDef.setParam("docNumber", documentNumber);
                    
                    if (string.IsNullOrEmpty(versionLabel))
                        queryDef.setParam("versionLabel", "null");
                    else
                        queryDef.setParam("versionLabel", versionLabel);
                    queryDef.setParam("typist", idPeople);
                    queryDef.setParam("checkoutdate", DocsPaDbManagement.Functions.Functions.ToDate(lockDate.ToString("dd/MM/yyyy"), false));
                    double one = 1;
                    queryDef.setParam("checkindate", DocsPaDbManagement.Functions.Functions.ToDate(lockDate.AddDays(one).ToString("dd/MM/yyyy"), false));
                    queryDef.setParam("comments", "null");
                    queryDef.setParam("checkouttime", DocsPaDbManagement.Functions.Functions.ToDate(lockDate.ToString("dd/MM/yyyy")));
                    queryDef.setParam("location", documentLocation);

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    int rowsAffected;

                    dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

                    retValue = (rowsAffected == 1);

                    if (retValue)
                    {
                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("HUMMY_U_MANUAL_CHECKINOUT_DOCUMENT_STATE");
                        queryDef.setParam("status", "3");
                        queryDef.setParam("docNumber", documentNumber);

                        commandText = queryDef.getSQL();
                        logger.Debug(commandText);

                        dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

                        retValue = (rowsAffected == 1);
                    }
                }
                catch (Exception ex)
                {
                    retValue = false;
                    logger.Debug("ManualLock ERROR:" + ex.Message);
                }
                finally
                {
                    if (retValue)
                        dbProvider.CommitTransaction();
                    else
                        dbProvider.RollbackTransaction();
                }
            }

            return retValue;
        }

		/// <summary>
		/// Rimozione manuale del blocco sul documento nel documentale hummingbird.
		/// 1. Cancellazione del record dalla tabella "CHECKOUT"
		/// 2. Impostazione del campo "STATUS" a 0 nella tabella "PROFILE" relativa
		///    al documento da sbloccare
		/// </summary>
		/// <param name="documentNumber"></param>
		/// <returns></returns>
		public static bool ManualUnlock(string documentNumber)
		{
			bool retValue=false;

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("HUMMY_D_MANUAL_CHECKOUT_DOCUMENT");
                    queryDef.setParam("docNumber", documentNumber);
                    
                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    int rowsAffected;
                    dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                    retValue = (rowsAffected == 1);

                    if (retValue)
                    {
                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("HUMMY_U_MANUAL_CHECKINOUT_DOCUMENT_STATE");
                        queryDef.setParam("status", "0");
                        queryDef.setParam("docNumber", documentNumber);

                        commandText = queryDef.getSQL();
                        logger.Debug(commandText);

                        dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

                        retValue = (rowsAffected == 1);
                    }
                }
                catch (Exception ex)
                {
                    retValue = false;
                    logger.Debug("ManualUnlock ERROR:" + ex.Message);
                }
                finally
                {
                    if (retValue)
                        dbProvider.CommitTransaction();
                    else
                        dbProvider.RollbackTransaction();
                }
            }

			return retValue;
		}
	}
}
