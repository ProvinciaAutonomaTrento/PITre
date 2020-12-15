using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO;
using DocsPaVO.utente;
using DocsPaVO.CheckInOut;
using DocsPaVO.documento;
using CheckInOutManagerDCTM = DocsPaDocumentale_DOCUMENTUM.Documentale.CheckInOutDocumentManager;
using CheckInOutManagerETDOCS = DocsPaDocumentale_ETDOCS.Documentale.CheckInOutDocumentManager;
using log4net;

namespace DocsPaDocumentale_PITRE.Documentale
{
    /// <summary>
    /// Classe per la gestione del checkIn/checkOut dell'ultima versione di un documento
    /// </summary>
    public class CheckInOutDocumentManager : ICheckInOutDocumentManager
    {
        private ILog logger = LogManager.GetLogger(typeof(CheckInOutDocumentManager));
        #region Ctros, variables, constants

        /// <summary>
        /// 
        /// </summary>
        private ICheckInOutDocumentManager _checkInOutDCTM = null;

        /// <summary>
        /// 
        /// </summary>
        private ICheckInOutDocumentManager _checkInOutETDOCS = null;

        /// <summary>
        /// 
        /// </summary>
        private InfoUtente _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public CheckInOutDocumentManager(InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Reperimento delle informazioni di stato di checkout del documento
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentNumber"></param>
        /// <returns></returns>
        /// <remarks>
        /// Le informazioni vengono reperite solamente in ETDOCS
        /// </remarks>
        public CheckOutStatus GetCheckOutStatus(string idDocument, string documentNumber)
        {
            return this.CheckInOutManagerETDOCS.GetCheckOutStatus(idDocument, documentNumber);
        }

        /// <summary>
        /// Verifica se il documento è in stato checkout
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentNumber"></param>
        /// <returns></returns>
        /// <remarks>
        /// Le informazioni vengono reperite solamente in ETDOCS
        /// </remarks>
        public bool IsCheckedOut(string idDocument, string documentNumber)
        {
            return this.CheckInOutManagerETDOCS.IsCheckedOut(idDocument, documentNumber);
        }

        /// <summary>
        /// Verifica se il documento è in stato checkout
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentNumber"></param>
        /// <param name="ownerUser"></param>
        /// <returns></returns>
        /// <remarks>
        /// Le informazioni vengono reperite solamente in ETDOCS
        /// </remarks>
        public bool IsCheckedOut(string idDocument, string documentNumber, out string ownerUser)
        {
            return this.CheckInOutManagerETDOCS.IsCheckedOut(idDocument, documentNumber, out ownerUser);
        }

        /// <summary>
        /// CheckOut del documento
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentNumber"></param>
        /// <param name="documentLocation"></param>
        /// <param name="machineName"></param>
        /// <param name="checkOutStatus"></param>
        /// <returns></returns>
        /// <remarks>
        /// Il blocco del documento viene impostato solamente in ETDOCS
        /// </remarks>
        public bool CheckOut(string idDocument, string documentNumber, string documentLocation, string machineName, out CheckOutStatus checkOutStatus)
        {
            return this.CheckInOutManagerETDOCS.CheckOut(idDocument, documentNumber, documentLocation, machineName, out checkOutStatus);
        }

        /// <summary>
        /// CheckIn del documento
        /// </summary>
        /// <param name="checkOutStatus"></param>
        /// <param name="content"></param>
        /// <param name="checkInComments"></param>
        /// <returns></returns>
        /// <remarks>
        /// Il documento è in checkout solo nel documentale ETDOCS,
        /// pertanto l'operazione di checkin, oltre ad effettuare il checkin in ETDOCS,
        /// dovrà aggiungere la nuova versione anche in DCTM. Ciò comporta
        /// l'utilizzo dei servizi di checkout e checkin del documentale DCTM.
        /// </remarks>
        public bool CheckIn(DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus, byte[] content, string checkInComments)
        {
            bool retValue = false;

            // Stato del checkout temporaneo in DCTM

            // Flag, se true indica che il documento è temporaneamente in checkout in DCTM
            bool hasCheckedOutDCTM = false;

            // Flag, se true indica che per il documento è stata creata una versione in DCTM;
            // in tal caso il flag 'hasCheckedOutDCTM' deve essere nello stato "false"
            bool hasCheckedInDCTM = false;

            // Informazioni di stato checkout del documento in DCTM
            DocsPaVO.CheckInOut.CheckOutStatus checkOutStatusDCTM = null;

            int nextVersion = this.GetNextVersionId(checkOutStatus.IDDocument);

            try
            {
                // CheckOut del documento in DCTM per la creazione della versione in DCTM, 
                if (this.CheckInOutManagerDCTM.CheckOut(checkOutStatus.IDDocument,
                                                        checkOutStatus.DocumentNumber,
                                                        checkOutStatus.DocumentLocation,
                                                        checkOutStatus.MachineName,
                                                        out checkOutStatusDCTM))
                {
                    hasCheckedOutDCTM = true;

                    // CheckIn del documento in DCTM
                    retValue = this.CheckInOutManagerDCTM.CheckIn(checkOutStatusDCTM, content, checkInComments);

                    if (retValue)
                    {
                        hasCheckedOutDCTM = false;
                        hasCheckedInDCTM = true;

                        if (this.PitreDualFileWritingMode)
                        {
                            // In modalità doppia scrittura, viene effettuato il CheckIn del documento in ETDOCS
                            // NB: L'implementazione del checkin in ETDOCS inserisce il file nel documentale
                            retValue = this.CheckInOutManagerETDOCS.CheckIn(checkOutStatus, content, checkInComments);
                        }
                        else
                        {
                            // Doppia scrittura non attiva

                            // Viene effettuato l'undocheckout del documento e inserita una nuova versione in ETDOCS
                            this.CheckInOutManagerETDOCS.UndoCheckOut(checkOutStatus);

                            // Creazione della versione del documento in ETDOCS
                            retValue = this.CreateDocumentVersion(checkOutStatus, checkInComments, content);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Si è verificato un errore nel CheckIn del documento nel documentale PITRE", ex);

                retValue = false;
            }
            finally
            {
                if (!retValue)
                {
                    try
                    {
                        // Operazioni di compensazione e ripristino in caso di errore 

                        if (hasCheckedOutDCTM && !hasCheckedInDCTM)
                        {
                            // Il documento è rimasto in checkout in DCTM, deve essere annullato
                            if (this.CheckInOutManagerDCTM.UndoCheckOut(checkOutStatusDCTM))
                            {
                                logger.Debug(string.Format("UndoCheckOut per il documento con DocNumber '{0}' in Documentum", checkOutStatus.DocumentNumber));
                            }
                        }
                        else if (!hasCheckedOutDCTM && hasCheckedInDCTM)
                        {
                            // Per il documento è stata creata una versione in DCTM, deve essere rimossa

                            DocsPaVO.documento.FileRequest versionToDelete = this.GetFileRequest(checkOutStatus);

                            bool isAcquired = this.IsAcquired(versionToDelete);

                            if (isAcquired)
                            {
                                versionToDelete.version = nextVersion.ToString();

                                IDocumentManager documentManager = new DocsPaDocumentale_DOCUMENTUM.Documentale.DocumentManager(this.InfoUtente);

                                if (documentManager.RemoveVersion(versionToDelete))
                                {
                                    logger.Debug(string.Format("Versione '{0}' per il documento con DocNumber '{0}' rimossa in Documentum", versionToDelete.version, versionToDelete.docNumber));
                                }
                            }
                        }
                    }
                    catch (Exception innerEx)
                    {
                        logger.Debug("Si è verificato un errore nelle operazioni di compensazione e ripristino del CheckIn del documento nel documentale PITRE", innerEx);
                    }
                }
            }

            return retValue;
        }

        /// <summary>
        /// UndoCheckOut del documento
        /// </summary>
        /// <param name="checkOutStatus"></param>
        /// <returns></returns>
        /// <remarks>
        /// L'operazione viene effettuata solamente nel documentale ETDOCS
        /// </remarks>
        public bool UndoCheckOut(DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus)
        {
            return this.CheckInOutManagerETDOCS.UndoCheckOut(checkOutStatus);
        }

        #endregion

        #region Protected methods



        /// <summary>
        /// 
        /// </summary>
        protected ICheckInOutDocumentManager CheckInOutManagerDCTM
        {
            get
            {
                if (this._checkInOutDCTM == null)
                    this._checkInOutDCTM = new CheckInOutManagerDCTM(this._infoUtente);
                return this._checkInOutDCTM;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected ICheckInOutDocumentManager CheckInOutManagerETDOCS
        {
            get
            {
                if (this._checkInOutETDOCS == null)
                    this._checkInOutETDOCS = new CheckInOutManagerETDOCS(this._infoUtente);
                return this._checkInOutETDOCS;
            }
        }

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
        /// Creazione di una nuova versione del documento
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="checkOutStatus"></param>
        /// <param name="checkedOutFile"></param>
        /// <param name="checkInComments"></param>
        /// <returns></returns>
        protected bool CreateDocumentVersion(CheckOutStatus checkOutStatus, string checkInComments, byte[] content)
        {
            bool retValue = false;

            // Reperimento dell'ultima versione del documento
            FileRequest fileRequest = this.GetFileRequest(checkOutStatus);

            retValue = this.AddCheckInVersion(this.InfoUtente, checkOutStatus, checkInComments, fileRequest, content);

            return retValue;
        }

        /// <summary>
        /// Reperiemento dell'oggetto FileRequest relativamente l'ultima versione del documento
        /// </summary>
        /// <param name="checkOutStatus"></param>
        /// <returns></returns>
        protected FileRequest GetFileRequest(CheckOutStatus checkOutStatus)
        {
            DocsPaDB.Query_DocsPAWS.CheckInOut checkInOutDb = new DocsPaDB.Query_DocsPAWS.CheckInOut();

            // Reperimento dell'ultima versione del documento
            return checkInOutDb.GetFileRequest(checkOutStatus.IDDocument);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        protected int GetNextVersionId(string idDocument)
        {
            DocsPaDB.Query_DocsPAWS.CheckInOut checkInOutDb = new DocsPaDB.Query_DocsPAWS.CheckInOut();

            return checkInOutDb.GetNextVersionId(idDocument);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        protected bool IsAcquired(DocsPaVO.documento.FileRequest fileRequest)
        {
            DocsPaDB.Query_DocsPAWS.CheckInOut checkInOutDb = new DocsPaDB.Query_DocsPAWS.CheckInOut();

            return checkInOutDb.IsAcquired(fileRequest);
        }

        /// <summary>
        /// Creazione dei metadati di una nuova versione di un documento in etdocs
        /// </summary>
        /// <param name="fileRequest">
        /// Rappresenta i metadati relativi all'ultima versione del documento
        /// </param>
        /// <param name="fileContent">
        /// Rappresenta il contenuto inviato per il checkin
        /// </param>
        /// <param name="fileExtension">
        /// Estensione del file
        /// </param>
        /// <returns></returns>
        /// <summary>
        /// Creazione dei metadati di una nuova versione di un documento in etdocs
        /// </summary>
        /// <param name="fileRequest">
        /// Rappresenta i metadati relativi all'ultima versione del documento
        /// </param>
        /// <param name="fileContent">
        /// Rappresenta il contenuto inviato per il checkin
        /// </param>
        /// <param name="fileExtension">
        /// Estensione del file
        /// </param>
        /// <returns></returns>
        protected virtual bool AddCheckInVersion(InfoUtente infoUtente, CheckOutStatus checkOutStatus, string checkInComments, FileRequest fileRequest, byte[] fileContent)
        {
            bool retValue = false;

            DocsPaDocumentale_ETDOCS.Documentale.DocumentManager documentManager = new DocsPaDocumentale_ETDOCS.Documentale.DocumentManager(infoUtente);

            DocsPaDB.Query_DocsPAWS.CheckInOut checkInOutDb = new DocsPaDB.Query_DocsPAWS.CheckInOut();

            if (checkInOutDb.IsAcquired(fileRequest))
            {
                // Se l'ultima versione del documento risulta acquisita già,
                // viene creata una nuova versione

                // Se per l'ultima versione del documento è stato acquisito un file,
                // viene creata nuova versione per il documento
                fileRequest = new FileRequest();
                fileRequest.docNumber = checkOutStatus.DocumentNumber;
                fileRequest.descrizione = checkInComments;
            }
            else
            {
                // Se l'ultima versione del documento non risulta ancora acquisita,
                // il file viene inserito in quest'ultima

                // Se per l'ultima versione del documento non è stato acquisito un file, il file viene acquisito per l'ultima versione
                fileRequest.descrizione = checkInComments;
                retValue = true;
            }

            if (fileContent.Length > 0)
            {
                // Il file risulta acquisito

                // Inserimento nuova versione in etdocs con flag "cha_da_inviare = 0"
                retValue = documentManager.AddVersion(fileRequest, false);

                string fileExtension = System.IO.Path.GetExtension(checkOutStatus.DocumentLocation).Replace(".", string.Empty);

                fileRequest.fileName = string.Format("{0}.{1}", fileRequest.versionId, fileExtension);
                fileRequest.fileSize = fileContent.Length.ToString();
                fileRequest.subVersion = "A";

                // Impostazione del file come inserito in etdocs
                String nomeOriginale = System.IO.Path.GetFileName(checkOutStatus.DocumentLocation);
                this.SetFileAsInserted(fileRequest, fileContent, nomeOriginale);
            }
            else
            {
                // Il file non risulta acquisito

                // Inserimento nuova versione in etdocs con flag "cha_da_inviare = 1"
                // in quanto nessun content è stato inviato per la versione
                retValue = documentManager.AddVersion(fileRequest, true);
            }

            return retValue;
        }

        /// <summary>
        /// Inserimento dei metadati di un file inserito in etdocs
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="fileContent"></param>
        /// <param name="fileExtension"></param>
        protected virtual void SetFileAsInserted(FileRequest fileRequest, byte[] fileContent,string originalFileName)
        {
            // Aggiornamento tabella COMPONENTS				
            string varImpronta = DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(fileContent);
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

            doc.UpdateVersion(fileRequest);
            doc.UpdateComponents(fileRequest.fileSize, varImpronta, fileRequest.versionId, fileRequest.docNumber);

            string fileExtension = null;
            try
            {
                fileExtension = System.IO.Path.GetExtension(fileRequest.fileName).Replace(".", string.Empty);

                doc.UpdateComponentsExt(fileExtension, fileRequest.versionId, fileRequest.docNumber);
            }
            catch (Exception ex)
            {
                logger.Debug("Si è verificato un errore nel reperimento del fileName da metodo GetExtension", ex);

            }

            DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
            documentale.UpdateFileName(fileRequest.fileName, fileRequest.versionId);
            DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
            documenti.UpdateComponentsOfn(originalFileName, fileRequest.versionId, fileRequest.docNumber);


            //Aggiornamento tabella PROFILE
            int version;
            if (Int32.TryParse(fileRequest.version, out version))
            {
                if (version > 0)
                    doc.SetImg(fileRequest.docNumber);
            }
        }

        /// <summary>
        /// Indica se è attiva la modalità di scrittura del file su entrambi i documentali
        /// </summary>
        protected bool PitreDualFileWritingMode
        {
            get
            {
                const string KEY = "PitreDualFileWritingMode";

                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[KEY]))
                    return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings[KEY]);
                else
                    return false;
            }
        }

        #endregion
    }
}