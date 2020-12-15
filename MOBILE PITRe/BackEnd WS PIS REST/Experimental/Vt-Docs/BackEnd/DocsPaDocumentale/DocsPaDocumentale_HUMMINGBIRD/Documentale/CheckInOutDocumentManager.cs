using System;
using System.Data;
using System.IO;
using DocsPaVO.CheckInOut;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.utente;
using DocsPaVO.documento;
using log4net;

namespace DocsPaDocumentale_HUMMINGBIRD.Documentale
{
	/// <summary>
	/// Gestione del checkin/checkout di documenti per il documentale HUMMINGBIRD
	/// </summary>
	public class CheckInOutDocumentManager : ICheckInOutDocumentManager
	{
        private ILog logger = LogManager.GetLogger(typeof(CheckInOutDocumentManager));
        /// <summary>
        /// 
        /// </summary>
        private InfoUtente _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        private string _library = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public CheckInOutDocumentManager(InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;
            this._library = DocsPaDB.Utils.Personalization.getInstance(infoUtente.idAmministrazione).getLibrary();
        }
	
		#region Public methods

		/// <summary>
		/// Reperimento delle informazioni di stato su un documento in stato checkedout
		/// </summary>
		/// <param name="idDocument"></param>
		/// <param name="documentNumber"></param>
		/// <returns></returns>
		public CheckOutStatus GetCheckOutStatus(string idDocument,string documentNumber)
		{
			CheckOutStatus status=null;

			try
			{
				DocsPaUtils.Query queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("CHECKINOUT_HUMMY_GET_DOCUMENT_STATUS");
				queryDef.setParam("documentNumber",documentNumber);

				string commandText=queryDef.getSQL();
				logger.Debug(commandText);

				using (DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider())
				{
					using (IDataReader reader=dbProvider.ExecuteReader(commandText))
					{
						if (reader.Read())
						{
							status=this.GetCheckOutInfo(reader);

							status.IDDocument=idDocument;
						}
					}
				}
			}
			catch (Exception ex)
			{
				logger.Debug(ex.Message,ex);

				throw new ApplicationException("Errore nel reperimento dello stato del documento in CheckOut. IDDocumento: " + idDocument,ex);
			}

			return status;
		}

		/// <summary>
		/// Annullamento dello stato CheckedOut per un documento
		/// </summary>
		/// <param name="checkOutInfo"></param>
		/// <returns></returns>
		public bool UndoCheckOut(CheckOutStatus checkOutStatus)
		{
            return LockDocumentManager.ManualUnlock(checkOutStatus.DocumentNumber);
		}

		/// <summary>
		/// CheckIn di un documento
		/// </summary>
		/// <param name="checkOutInfo">Informazioni di stato sul documento in checkOut</param>
		/// <returns></returns>
        public bool CheckIn(CheckOutStatus checkOutStatus, byte[] content, string checkInComments)
		{
            bool retValue = LockDocumentManager.ManualUnlock(checkOutStatus.DocumentNumber);

            if (retValue)
                // Creazione della versione del documento
                retValue = this.CreateDocumentVersion(checkOutStatus, content, checkInComments, this.InfoUtente);

            return retValue;
		}

		/// <summary>
		/// Verifica se un documento è in stato checkedout
		/// </summary>
		/// <param name="idDocument"></param>
		/// <param name="checkOutInfo">Informazioni sullo stato del checkout</param>
		/// <returns></returns>
        public bool IsCheckedOut(string idDocument, string documentNumber, out string ownerUser)
		{
            bool retValue=false;
            ownerUser = string.Empty;

			try
			{
				DocsPaUtils.Query queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("CHECKINOUT_HUMMY_IS_CHECKED_OUT");
				queryDef.setParam("documentNumber",documentNumber);

				string commandText=queryDef.getSQL();
				logger.Debug(commandText);

				using (DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider())
				{
					using (IDataReader reader=dbProvider.ExecuteReader(commandText))
					{
						if (reader.Read())
						{
                            retValue = true;

                            ownerUser = reader.GetValue(reader.GetOrdinal("USER_NAME")).ToString();
						}
					}
				}
			}
			catch (Exception ex)
			{
				logger.Debug(ex.Message,ex);

				throw new ApplicationException("Errore nella verifica dello stato del documento estratto. IDDocumento: " + idDocument,ex);
			}

			return retValue;
		}

		/// <summary>
		/// Verifica se un documento è in stato checkedout
		/// </summary>
		/// <param name="idDocument"></param>
		/// <returns></returns>
		public bool IsCheckedOut(string idDocument,string documentNumber)
		{
            string ownerUser;
            return this.IsCheckedOut(idDocument, documentNumber, out ownerUser);
		}

		/// <summary>
		/// CheckOut di un documento
		/// </summary>
		/// <param name="checkOutInfo"></param>
		/// <returns></returns>
        public bool CheckOut(string idDocument, string documentNumber, string documentLocation, string machineName, out CheckOutStatus checkOutStatus)
		{
            checkOutStatus = null;
			bool retValue=false;

			try
			{
				// Impostazione del CheckOut nel documentale
				retValue = LockDocumentManager.ManualLock(documentNumber, string.Empty, documentLocation, this.InfoUtente.idPeople, this.InfoUtente.dst, this.Library);

				if (retValue)
					checkOutStatus=this.GetCheckOutStatus(idDocument, documentNumber);
			}
			catch (Exception ex)
			{
				logger.Debug(ex.Message,ex);

				throw new ApplicationException("Errore nel checkout del documento. IDDocumento: " + idDocument, ex);
			}

			return retValue;
		}

		#endregion

        #region Protected methods

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
        /// 
        /// </summary>
        protected string Library
        {
            get
            {
                return this._library;
            }
        }

        /// <summary>
        /// Creazione di una nuova versione del documento
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="checkOutStatus"></param>
        /// <param name="checkedOutFile"></param>
        /// <param name="checkInComments"></param>
        /// <returns></returns>
        protected bool CreateDocumentVersion(CheckOutStatus checkOutStatus, byte[] checkedOutFileContent, string checkInComments, InfoUtente checkOutOwner)
        {
            bool retValue = false;

            DocsPaDB.Query_DocsPAWS.CheckInOut checkInOutDb = new DocsPaDB.Query_DocsPAWS.CheckInOut();

            // Reperimento dell'ultima versione del documento
            FileRequest fileRequest = checkInOutDb.GetFileRequest(checkOutStatus.IDDocument);

            FileDocumento fileDocument = CreateFileDocument(checkOutStatus.DocumentLocation, checkedOutFileContent);

            bool isAcquired = checkInOutDb.IsAcquired(fileRequest);

            if (isAcquired)
            {
                // Se per l'ultima versione del documento è stato acquisito un file,
                // viene creata nuova versione per il documento
                FileRequest newFileRequest = new FileRequest();
                newFileRequest.fileName = checkOutStatus.DocumentLocation;
                newFileRequest.docNumber = checkOutStatus.DocumentNumber;
                newFileRequest.subVersion = "!";
                newFileRequest.fileSize = checkedOutFileContent.Length.ToString();
                newFileRequest.version = (Convert.ToInt32(fileRequest.version) + 1).ToString();
                newFileRequest.versionLabel = newFileRequest.version;

                // Impostazione degli eventuali commenti da aggiungere alla versione
                newFileRequest.descrizione = checkInComments;
                newFileRequest.applicazione = this.GetApplication(fileDocument.estensioneFile);

                fileRequest = newFileRequest;
            }
            else
            {
                // Se per l'ultima versione del documento non è stato acquisito un file,
                // il file viene acquisito per l'ultima versione
                fileRequest.fileName = fileDocument.name;

                // Impostazione degli eventuali commenti da aggiungere alla versione
                fileRequest.descrizione = checkInComments;

                fileRequest.applicazione = this.GetApplication(fileDocument.estensioneFile);

                retValue = true;
            }

            string docNumber = fileRequest.docNumber;
            string version_id = fileRequest.versionId;
            string version = fileRequest.version;
            string subVersion = fileRequest.subVersion;
            string versionLabel = fileRequest.versionLabel;

            DocumentManager documentManager = new DocumentManager(this.InfoUtente);
            retValue = documentManager.AddVersion(fileRequest, isAcquired);

            if (retValue && !isAcquired)
            {
                retValue = documentManager.ModifyExtension(ref fileRequest, docNumber, version_id, version, subVersion, versionLabel);
            }

            if (retValue && fileDocument != null &&
                fileDocument.content != null &&
                fileDocument.content.Length > 0)
            {
                // Inserimento del nuovo file per la versione
                retValue = documentManager.PutFile(fileRequest, fileDocument, fileDocument.estensioneFile);
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        protected DocsPaVO.documento.Applicazione GetApplication(string extension)
        {
            DocsPaVO.documento.Applicazione app = new DocsPaVO.documento.Applicazione();
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            doc.GetExt(extension, ref app);
            return app;
        }

        /// <summary>
        /// Creazione di un oggetto FileDocumento a partire dal percorso del file
        /// </summary>
        /// <param name="documentLocation"></param>
        /// <returns></returns>
        protected FileDocumento CreateFileDocument(string documentLocation, byte[] content)
        {
            FileDocumento fileDocument = new FileDocumento();

            FileInfo fileInfo = new FileInfo(documentLocation);
            fileDocument.fullName = fileInfo.FullName;
            fileDocument.name = fileInfo.Name;
            fileDocument.estensioneFile = fileInfo.Extension.Replace(".", string.Empty);

            if (content != null)
            {
                fileDocument.content = content;
                fileDocument.length = content.Length;
            }
            fileDocument.path = this.GetDocRootPath();

            return fileDocument;
        }

        /// <summary>
        /// Reperimento percorso principale del documentale
        /// </summary>
        /// <returns></returns>
        protected string GetDocRootPath()
        {
            return System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"];
        }

        #endregion

        #region Private methods

        /// <summary>
		/// Creazione oggetto "CheckOutInfo" da datareader
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		private CheckOutStatus GetCheckOutInfo(IDataReader reader)
		{
			CheckOutStatus retValue=new CheckOutStatus();

            retValue.ID = reader.GetValue(reader.GetOrdinal("SYSTEM_ID")).ToString();
			retValue.DocumentNumber=reader.GetValue(reader.GetOrdinal("DOCUMENT_NUMBER")).ToString();
            retValue.Segnature = reader.GetValue(reader.GetOrdinal("VAR_SEGNATURA")).ToString();
			retValue.IDUser=reader.GetValue(reader.GetOrdinal("ID_USER")).ToString();
			retValue.UserName=reader.GetValue(reader.GetOrdinal("USER_NAME")).ToString();
			retValue.IDRole=string.Empty; // ID del ruolo non gestito dal documentale Hummingbird
			retValue.CheckOutDate=reader.GetDateTime(reader.GetOrdinal("CHECK_OUT_DATE"));
			retValue.DocumentLocation=reader.GetValue(reader.GetOrdinal("DOCUMENT_LOCATION")).ToString();
			retValue.MachineName=string.Empty; // Nome macchina non gestito dal documente Hummingbird

			return retValue;
		}

		#endregion
	}
}
