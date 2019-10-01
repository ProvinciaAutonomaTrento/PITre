using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO;
using DocsPaVO.utente;
using DocsPaVO.CheckInOut;
using DocsPaDocumentale_DOCUMENTUM.DocsPaServices;
using DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes;
using DocsPaDocumentale_DOCUMENTUM.DctmServices;
using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.Services.Core;
using Emc.Documentum.FS.DataModel.Core.Properties;
using Emc.Documentum.FS.DataModel.Core.Profiles;
using Emc.Documentum.FS.DataModel.Core.Content;
using log4net;

namespace DocsPaDocumentale_DOCUMENTUM.Documentale
{
    /// <summary>
    /// Classe per la gestione del checkIn/checkOut dell'ultima versione di un documento
    /// </summary>
    public class CheckInOutDocumentManager : ICheckInOutDocumentManager
    {
        private ILog logger = LogManager.GetLogger(typeof(CheckInOutDocumentManager));
        #region Ctros, variables, constants

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

        #region Public methods

        /// <summary>
        /// Reperimento dei metadati sullo stato del checkout del documento
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentNumber"></param>
        /// <returns></returns>
        public CheckOutStatus GetCheckOutStatus(string idDocument, string documentNumber)
        {
            CheckOutStatus retValue = null;

            CheckoutInfo dctmCheckOutInfo = this.GetDctmCheckOutInfo(documentNumber);

            if (dctmCheckOutInfo.IsCheckedOut)
            {
                string dctmId = ((ObjectId)dctmCheckOutInfo.Identity.Value).Id;

                retValue = this.GetCheckOutStatus(dctmId, idDocument, documentNumber);
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentNumber"></param>
        /// <returns></returns>
        public bool IsCheckedOut(string idDocument, string documentNumber)
        {
            string ownerUser;
            return this.IsCheckedOut(idDocument, documentNumber, out ownerUser);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentNumber"></param>
        /// <param name="checkOutStatus"></param>
        /// <returns></returns>
        public bool IsCheckedOut(string idDocument, string documentNumber, out string ownerUser)
        {
            ownerUser = string.Empty;
            CheckoutInfo info = this.GetDctmCheckOutInfo(documentNumber);

            if (info != null)
            {
                ownerUser = info.UserName;
                return info.IsCheckedOut;
            }
            else
                return false;
        }

        /// <summary>
        /// Checkout di un documento
        /// </summary>
        /// <param name="checkOutStatus">Metadati relativi allo stato del documento in checkout</param>
        /// <param name="user"></param>
        /// <param name="library"></param>
        /// <returns></returns>
        public bool CheckOut(string idDocument, string documentNumber, string documentLocation, string machineName, out CheckOutStatus checkOutStatus)
        {
            checkOutStatus = null;

            // Determina se il documento da bloccare è di tipo stampa registro
            bool isStampaRegistro = (DocsPaQueryHelper.isStampaRegistro(documentNumber));

            bool retValue = this.SaveCheckOutDocumentProperties(string.Empty, documentNumber, documentLocation, machineName);

            try
            {
                if (retValue)
                {
                    ObjectIdentity identity = null;

                    // Reperimento identity del documento da bloccare
                    if (isStampaRegistro)
                        identity = Dfs4DocsPa.getDocumentoStampaRegistroIdentityByDocNumber(documentNumber);
                    else
                        identity = Dfs4DocsPa.getDocumentoIdentityByDocNumber(documentNumber);

                    ObjectIdentitySet identitySet = new ObjectIdentitySet();
                    identitySet.Identities.Add(identity);

                    OperationOptions opts = null;

                    if (DocsPaQueryHelper.getCountAllegati(idDocument) > 0)
                    {
                        // Se il documento contiene allegati, è sicuramente un virtual document
                        CheckoutProfile checkOutProfile = new CheckoutProfile();

                        // Ulteriore verifica se il documento è un allegato o un documento principale
                        checkOutProfile.CheckoutOnlyVDMRoot = !DocsPaQueryHelper.isAllegatoProfilato(documentNumber);

                        opts = new OperationOptions();
                        opts.CheckoutProfile = checkOutProfile;
                    }

                    IVersionControlService service = this.GetServiceInstance<IVersionControlService>(false);
                    DataPackage dataPackage = service.Checkout(identitySet, opts);

                    // Reperimento ObjectId della versione in checkout
                    ObjectId objectId = (ObjectId)dataPackage.DataObjects[0].Identity.Value;
                    checkOutStatus = this.GetCheckOutStatus(objectId.Id, idDocument, documentNumber);

                    //if (DocsPaQueryHelper.getCountAllegati(idDocument) > 0)
                    //{
                    //    // Workaround per gli allegati
                    //    this.UndoCheckOutAllegati(documentNumber);
                    //}

                    retValue = true;

                    logger.Debug(string.Format("Documentum.CheckOut: effettuato il checkout del documento con id {0} e docnumber {1}", idDocument, documentNumber));
                }
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in Documentum.CheckOut: " + ex.ToString());
            }

            return retValue;
        }

        /// <summary>
        /// Checkin di un documento in stato checkout
        /// </summary>
        /// <param name="checkOutStatus"></param>
        /// <param name="library"></param>
        /// <returns></returns>
        public bool CheckIn(DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus, byte[] content, string checkInComments)
        {
            bool retValue = false;

            try
            {
                // Creazione di un nuovo DataObject che rappresenta il documento da sbloccare
                DataObject dataObject = new DataObject();
                dataObject.Type = ObjectTypes.DOCUMENTO;

                // Reperimento identity del documento da sbloccare
                if (DocsPaQueryHelper.isStampaRegistro(checkOutStatus.DocumentNumber))
                    dataObject.Identity = Dfs4DocsPa.getDocumentoStampaRegistroIdentityByDocNumber(checkOutStatus.DocumentNumber);
                else
                    dataObject.Identity = Dfs4DocsPa.getDocumentoIdentityByDocNumber(checkOutStatus.DocumentNumber);

                List<Property> propertyList = new List<Property>();

                // Impostazione numero versione
                propertyList.Add(new NumberProperty(TypeDocumento.NUMERO_VERSIONE, DocsPaQueryHelper.getDocumentNextVersionId(checkOutStatus.IDDocument)));

                // Rimozione valore proprietà p3_locked_filepath
                propertyList.Add(new StringProperty(TypeDocumento.CHECKOUT_LOCAL_FILE_PATH, string.Empty));

                // Rimozione valore proprietà p3_locked_file_machinename
                propertyList.Add(new StringProperty(TypeDocumento.CHECKOUT_MACHINE_NAME, string.Empty));

                dataObject.Properties = new PropertySet();
                dataObject.Properties.Properties.AddRange(propertyList);

                // Temporaneo, inserimento contentuto file
                OperationOptions opts = new OperationOptions();

                CheckinProfile checkInProfile = new CheckinProfile();
                checkInProfile.MakeCurrent = true;
                checkInProfile.DeleteLocalFileHint = true;

                opts.Profiles.Add(checkInProfile);

                // Creazione di un nuovo oggetto BinaryContent
                BinaryContent binaryContent = new BinaryContent();
                binaryContent.Value = content;

                string ext = System.IO.Path.GetExtension(checkOutStatus.DocumentLocation);
                if (ext.StartsWith("."))
                    ext = ext.Substring(1);
                string fileFormat = DfsHelper.getDctmFileFormat(this.GetServiceInstance<IQueryService>(false), ext);

                binaryContent.Format = fileFormat;

                dataObject.Contents.Add(binaryContent);

                DataPackage dataPackage = new DataPackage(dataObject);
                dataPackage.RepositoryName = DctmConfigurations.GetRepositoryName();

                IVersionControlService service = this.GetServiceInstance<IVersionControlService>(false);

                VersionStrategy strategy = VersionStrategy.IMPLIED;
                if (!DocsPaQueryHelper.isDocumentAcquisito(checkOutStatus.IDDocument))
                    strategy = VersionStrategy.SAME_VERSION;

                dataPackage = service.Checkin(dataPackage, strategy, false, null, opts);

                retValue = (dataPackage.DataObjects.Count > 0);

                if (retValue)
                    logger.Debug(string.Format("Documentum.CheckIn: effettuato il checkin del documento con id {0} e docnumber {1}", checkOutStatus.IDDocument, checkOutStatus.DocumentNumber));
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in Documentum.CheckIn: " + ex.ToString());
            }

            return retValue;
        }

        /// <summary>
        /// Implementazione di un workaround per il checkin dei singoli allegati di un documento (se presenti)
        /// </summary>
        /// <param name="docNumber"></param>
        protected void UndoCheckOutAllegati(string docNumber)
        {
            ObjectIdentitySet identitySet = new ObjectIdentitySet();
            identitySet.Identities.AddRange(Dfs4DocsPa.getAllegatiDocumentoIdentities(this.GetServiceInstance<IQueryService>(false), docNumber));
            if (identitySet.Identities.Count > 0)
                this.GetServiceInstance<IVersionControlService>(false).CancelCheckout(identitySet);
        }

        /// <summary>
        /// Rimozione valori attributi relativi allo stato checkout
        /// </summary>
        /// <param name="checkOutStatus"></param>
        protected virtual void ClearCheckOutStatusObject(DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus)
        {
            checkOutStatus.DocumentLocation = string.Empty;
            checkOutStatus.MachineName = string.Empty;
            checkOutStatus.CheckOutDate = DateTime.MinValue;
        }

        /// <summary>
        /// UndoCheckout di un documento in stato checkout
        /// </summary>
        /// <param name="checkOutStatus"></param>
        /// <param name="library"></param>
        /// <returns></returns>
        public bool UndoCheckOut(DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus)
        {
            bool retValue = false;

            try
            {
                ObjectIdentity identity = null;

                // Reperimento identity del documento da sbloccare
                if (DocsPaQueryHelper.isStampaRegistro(checkOutStatus.DocumentNumber))
                    identity = Dfs4DocsPa.getDocumentoStampaRegistroIdentityByDocNumber(checkOutStatus.DocumentNumber);
                else
                    identity = Dfs4DocsPa.getDocumentoIdentityByDocNumber(checkOutStatus.DocumentNumber);

                ObjectIdentitySet identitySet = new ObjectIdentitySet();
                identitySet.Identities.Add(identity);
                // Reperimento degli ObjectIdentity per ciascun allegato del documento
                identitySet.Identities.AddRange(Dfs4DocsPa.getAllegatiDocumentoIdentities(this.GetServiceInstance<IQueryService>(false), checkOutStatus.DocumentNumber));

                IVersionControlService service = this.GetServiceInstance<IVersionControlService>(false);
                service.CancelCheckout(identitySet);

                retValue = true;

                if (retValue)
                {
                    this.ClearCheckOutStatusObject(checkOutStatus);

                    retValue = this.SaveCheckOutDocumentProperties(checkOutStatus);

                    if (retValue)
                        logger.Debug(string.Format("Documentum.UndoCheckOut: effettuato l'undocheckout del documento con id {0} e docnumber {1}", checkOutStatus.IDDocument, checkOutStatus.DocumentNumber));
                }
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in Documentum.UndoCheckOut: " + ex.ToString());
            }

            return retValue;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Reperimento istanza servizio
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asSuperUser">
        /// Se true, si collega con le credenziali da superamministratore
        /// </param>
        /// <returns></returns>
        protected virtual T GetServiceInstance<T>(bool asSuperUser)
        {
            string dst = string.Empty;

            if (asSuperUser)
                dst = UserManager.ImpersonateSuperUser();
            else
                dst = this.InfoUtente.dst;

            return DctmServiceFactory.GetServiceInstance<T>(dst);
        }

        /// <summary>
        /// Caricamento dei valori delle properties relative allo stato checkout per il documento
        /// </summary>
        /// <param name="documentNumber"></param>
        /// <param name="isStampaRegistro"></param>
        /// <returns></returns>
        protected virtual PropertySet LoadCheckOutProperties(string documentNumber, bool isStampaRegistro)
        {
            ObjectIdentity identity = null;

            if (isStampaRegistro)
                identity = Dfs4DocsPa.getDocumentoStampaRegistroIdentityByDocNumber(documentNumber);
            else
                identity = Dfs4DocsPa.getDocumentoIdentityByDocNumber(documentNumber);

            List<string> filters = new List<string>();
            filters.Add("r_object_id");
            filters.Add("object_name");
            filters.Add("r_lock_owner");
            filters.Add("r_lock_date");
            filters.Add(TypeDocumento.CHECKOUT_LOCAL_FILE_PATH);
            filters.Add(TypeDocumento.CHECKOUT_MACHINE_NAME);

            if (!isStampaRegistro)
            {
                // Caricamento informazioni di checkout specifiche per documenti non di tipo stampa registro
                filters.Add(TypeDocumento.TIPO_PROTOCOLLO);
            }

            IObjectService service = this.GetServiceInstance<IObjectService>(true);
            DataObject dataObject = DfsHelper.getAllPropsAndFolders(service, identity, filters, false);

            return dataObject.Properties;
        }

        /// <summary>
        /// Reperimento dei metadati da documentum relativamente allo stato checkedout del documento 
        /// </summary>
        /// <param name="documentNumber"></param>
        /// <returns></returns>
        protected virtual CheckoutInfo GetDctmCheckOutInfo(string documentNumber)
        {
            ObjectIdentity identity = null;

            // Reperimento identity del documento da bloccare
            if (DocsPaQueryHelper.isStampaRegistro(documentNumber))
                identity = Dfs4DocsPa.getDocumentoStampaRegistroIdentityByDocNumber(documentNumber);
            else
                identity = Dfs4DocsPa.getDocumentoIdentityByDocNumber(documentNumber);

            if (identity.ValueType == ObjectIdentityType.QUALIFICATION)
                logger.Debug(((Qualification)identity.Value).GetValueAsString());

            ObjectIdentitySet identitySet = new ObjectIdentitySet();
            identitySet.Identities.Add(identity);

            //IVersionControlService service = this.GetVersionServiceInstance();
            IVersionControlService service = this.GetServiceInstance<IVersionControlService>(true);
            List<CheckoutInfo> checkOutInfoList = service.GetCheckoutInfo(identitySet);

            if (checkOutInfoList != null && checkOutInfoList.Count > 0)
                return checkOutInfoList[0];
            else
                return null;
        }

        /// <summary>
        /// Reperimento dei metadati sullo stato del checkout del documento
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentNumber"></param>
        /// <returns></returns>
        protected virtual CheckOutStatus GetCheckOutStatus(string dctmId, string idDocument, string documentNumber)
        {
            CheckOutStatus retValue = null;

            bool isStampaRegistro = (DocsPaQueryHelper.isStampaRegistro(documentNumber));

            PropertySet propertySet = this.LoadCheckOutProperties(documentNumber, isStampaRegistro);

            // Reperimento nome dell'utente che ha bloccato il documento
            string lockOwner = propertySet.Get("r_lock_owner").GetValueAsString();

            if (!string.IsNullOrEmpty(lockOwner))
            {
                // Se il documento è in stato checkout, creazione dell'oggetto "CheckOutStatus"
                retValue = new CheckOutStatus();
                retValue.ID = dctmId;
                retValue.UserName = lockOwner;
                retValue.IDDocument = idDocument;
                retValue.DocumentNumber = documentNumber;

                DateProperty checkOutDate = (DateProperty)propertySet.Get("r_lock_date");
                retValue.CheckOutDate = checkOutDate.Value.Value;

                // Attributi custom che è stato necessario aggiungere in documentum
                retValue.DocumentLocation = propertySet.Get(TypeDocumento.CHECKOUT_LOCAL_FILE_PATH).GetValueAsString();
                retValue.MachineName = propertySet.Get(TypeDocumento.CHECKOUT_MACHINE_NAME).GetValueAsString();

                if (!isStampaRegistro)
                {
                    // Reperimento segnatura, solo se il documento è un protocollo
                    string tipoProto = propertySet.Get(TypeDocumento.TIPO_PROTOCOLLO).GetValueAsString();
                    if (tipoProto.ToUpper() != "G")
                        retValue.Segnature = propertySet.Get("object_name").GetValueAsString();

                }
            }

            return retValue;
        }

        /// <summary>
        /// Aggiornamento delle proprietà dell'oggetto "p3_document" 
        /// relative al documento che dovrà essere impostato in checkout
        /// </summary>
        /// <param name="checkOutStatus"></param>
        /// <returns></returns>
        private bool SaveCheckOutDocumentProperties(DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus)
        {
            bool retValue = false;

            try
            {
                retValue = this.SaveCheckOutDocumentProperties(checkOutStatus.ID, checkOutStatus.DocumentNumber, checkOutStatus.DocumentLocation, checkOutStatus.MachineName);
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in Documentum. SaveCheckOutDocumentProperties: " + ex.ToString());
            }

            return retValue;
        }

        /// <summary>
        /// Aggiornamento delle proprietà dell'oggetto "p3_document" 
        /// </summary>
        /// <param name="idCheckOut"></param>
        /// <param name="documentNumber"></param>
        /// <param name="machineName"></param>
        /// <param name="documentLocation"></param>
        /// <returns></returns>
        private bool SaveCheckOutDocumentProperties(string idCheckOut, string documentNumber, string documentLocation, string machineName)
        {
            bool retValue = false;

            try
            {
                // Determina se il documento da bloccare è di tipo stampa registro
                bool isStampaRegistro = (DocsPaQueryHelper.isStampaRegistro(documentNumber));

                ObjectIdentity identity = null;

                if (!string.IsNullOrEmpty(idCheckOut))
                    identity = DfsHelper.createObjectIdentityObjId(idCheckOut);
                else
                {
                    if (DocsPaQueryHelper.isStampaRegistro(documentNumber))
                        identity = Dfs4DocsPa.getDocumentoStampaRegistroIdentityByDocNumber(documentNumber);
                    else
                        identity = Dfs4DocsPa.getDocumentoIdentityByDocNumber(documentNumber);
                }

                DataObject dataObject = new DataObject(identity);
                dataObject.Properties = new PropertySet();

                List<Property> propertyList = new List<Property>();
                propertyList.Add(new StringProperty(TypeDocumento.CHECKOUT_LOCAL_FILE_PATH, documentLocation));
                propertyList.Add(new StringProperty(TypeDocumento.CHECKOUT_MACHINE_NAME, machineName));
                dataObject.Properties.Properties.AddRange(propertyList);

                DataPackage dataPackage = new DataPackage(dataObject);
                dataPackage.RepositoryName = dataObject.Identity.RepositoryName;

                IObjectService service = this.GetServiceInstance<IObjectService>(false);
                dataPackage = service.Update(dataPackage, null);

                retValue = (dataPackage.DataObjects.Count > 0);
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in Documentum. SaveCheckOutDocumentProperties: " + ex.ToString());
            }

            return retValue;
        }

        /// <summary>
        /// Credenziali dell'utente connesso a documentum
        /// </summary>
        protected InfoUtente InfoUtente
        {
            get
            {
                return this._infoUtente;
            }
        }

        #endregion
    }
}