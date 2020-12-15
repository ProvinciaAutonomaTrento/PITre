using System;
using System.Data;
using System.Collections;
using DocsPaVO.utente;
using DocsPaVO.CheckInOut;
using DocsPaDocumentale.Interfaces;
using log4net;

namespace DocsPaDocumentale_HUMMINGBIRD.Documentale
{
	/// <summary>
	/// Gestione del checkin/checkout di documenti per l'amministrazione
	/// per il documentale HUMMINGBIRD
	/// </summary>
	public class CheckInOutAdminDocumentManager : ICheckInOutAdminDocumentManager
	{
        private ILog logger = LogManager.GetLogger(typeof(CheckInOutAdminDocumentManager));
        /// <summary>
        /// 
        /// </summary>
        private DocsPaVO.utente.InfoUtente _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public CheckInOutAdminDocumentManager(DocsPaVO.utente.InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;
        }

		#region Public methods

		/// <summary>
		/// Reperimento delle informazioni di stato sui documenti in stato checkedout 
		/// </summary>
        /// <param name="idAmministration"></param>
		/// <returns></returns>
        public CheckOutStatus[] GetCheckOutStatusDocuments(string idAmministration)
		{	
			DocsPaUtils.Query queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("CHECKINOUT_HUMMY_GET_CHECKED_OUT_DOCUMENTS");
            queryDef.setParam("idAmm", idAmministration);

			return this.GetCheckOutStatusDocuments(queryDef);
		}

		/// <summary>
		/// Reperimento delle informazioni di stato sui documenti in stato checkedout
		/// relativamente ad un utente
		/// </summary>
        /// <param name="idAmministration"></param>
		/// <param name="idUser"></param>
		/// <returns></returns>
		public CheckOutStatus[] GetCheckOutStatusDocuments(string idAmministration, string idUser)
		{
			DocsPaUtils.Query queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("CHECKINOUT_HUMMY_GET_CHECKED_OUT_DOCUMENTS_USER");
            queryDef.setParam("idUser", idUser);
            queryDef.setParam("idAmm", idAmministration);
            
			return this.GetCheckOutStatusDocuments(queryDef);
		}

		/// <summary>
		/// Annullamento del blocco del documento nel documentale
		/// </summary>
		/// <param name="adminInfoUtente"></param>
		/// <param name="checkOutAdminStatus"></param>
		/// <returns></returns>
		public bool ForceUndoCheckOut(CheckOutStatus checkOutAdminStatus)
		{
			if (this.CanForceUndoCheckOut())
			{
				string library=this.GetLibrary(this.InfoUtente.idAmministrazione);

				return LockDocumentManager.UnLock(checkOutAdminStatus.DocumentNumber, this.InfoUtente.dst, library);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Verifica se, per il documentale, è possibile annullare il blocco
		/// </summary>
		/// <param name="adminInfoUtente"></param>
		/// <returns></returns>
		public bool CanForceUndoCheckOut()
		{
			return true;
		}

		#endregion

		#region Private methods

        /// <summary>
        /// 
        /// </summary>
        protected InfoUtente InfoUtente
        {
            get
            {
                return this._infoUtente;
            }
        }

		/// <summary>
		/// Creazione oggetto "CheckOutAdminStatus" da datareader
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		private CheckOutStatus GetCheckOutAdminStatus(IDataReader reader)
		{
			CheckOutStatus retValue=new CheckOutStatus();
            
            retValue.ID = reader.GetValue(reader.GetOrdinal("ID_STATUS")).ToString();
			retValue.IDDocument=reader.GetValue(reader.GetOrdinal("ID_DOCUMENT")).ToString();
			retValue.DocumentNumber=reader.GetValue(reader.GetOrdinal("DOCUMENT_NUMBER")).ToString();
            retValue.Segnature = reader.GetValue(reader.GetOrdinal("VAR_SEGNATURA")).ToString();
			retValue.UserName=reader.GetValue(reader.GetOrdinal("USER_ID")).ToString();			
			retValue.CheckOutDate=reader.GetDateTime(reader.GetOrdinal("CHECK_OUT_DATE"));
			retValue.DocumentLocation=reader.GetValue(reader.GetOrdinal("DOCUMENT_LOCATION")).ToString();
			retValue.MachineName=string.Empty; // Nome macchina non gestito dal documentale hummingbird
			
			return retValue;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queryDef"></param>
		/// <returns></returns>
		private CheckOutStatus[] GetCheckOutStatusDocuments(DocsPaUtils.Query queryDef)
		{
			ArrayList retValue=new ArrayList();

			string commandText=queryDef.getSQL();
			logger.Debug(commandText);

			try
			{
				using (DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider())
				{
					using (IDataReader reader=dbProvider.ExecuteReader(commandText))
					{
						while (reader.Read())
							retValue.Add(this.GetCheckOutAdminStatus(reader));
					}
				}
			}
			catch (Exception ex)
			{
				logger.Debug(ex.Message);

				throw ex;
			}

			return (CheckOutStatus[]) retValue.ToArray(typeof(CheckOutStatus));
		}

		/// <summary>
		/// Reperimento nome libreria per l'amministrazione richiesta
		/// </summary>
		/// <param name="idAmministrazione"></param>
		/// <returns></returns>
		private string GetLibrary(string idAmministrazione)
		{
			return DocsPaDB.Utils.Personalization.getInstance(idAmministrazione).getLibrary();
		}

		#endregion
	}
}