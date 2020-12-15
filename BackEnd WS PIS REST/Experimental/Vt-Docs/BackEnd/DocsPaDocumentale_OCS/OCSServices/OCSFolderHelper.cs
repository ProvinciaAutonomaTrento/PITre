using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Data;
using DocsPaDocumentale.Interfaces;
using DocsPaVO;
using DocsPaVO.documento;
using DocsPaVO.utente;
using DocsPaUtils.LogsManagement;
using DocsPaDocumentale_OCS.CorteContentServices;

namespace DocsPaDocumentale_OCS.OCSServices
{
    /// <summary>
    /// Helper class per la gestione dei fascicoli in OCS
    /// </summary>
    public class OCSFolderHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="infoUtente"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        public static long getFolderIdByName(string name, DocsPaVO.utente.InfoUtente infoUtente, bool create)
        {
            long folderId = -1;

            CorteContentServices.FolderNameSearchRequestType folderReq = new CorteContentServices.FolderNameSearchRequestType();
            folderReq.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
            folderReq.folder = new DocsPaDocumentale_OCS.CorteContentServices.FolderNameSearchType();
            folderReq.folder.info = new DocsPaDocumentale_OCS.CorteContentServices.InfoNameType();

            int pos = name.LastIndexOf("/");
            if (pos > -1)
            {
                folderReq.folder.info.location = name.Substring(0, pos);
                folderReq.folder.info.name = name.Substring(pos + 1);
            }

            FolderManagementSOAPHTTPBinding wsFolder = OCSServiceFactory.GetDocumentServiceInstance<FolderManagementSOAPHTTPBinding>();
            CorteContentServices.ItemIdResponseType response = wsFolder.GetFolderByName(folderReq);

            // non posso lanciare l'eccezione perchè se il folder non esiste devo crearlo
            //OCSUtils.throwExceptionIfInvalidResult(response.result);

            if (OCSUtils.isValidServiceResult(response.result))
            {
                folderId = response.itemId;
            }
            else
            {
                // Se il folder non esiste bisogna crearlo (se richiesto)
                if (create && response.result.code == OCSResultTypeCodes.PATH_ITEM_NOT_VALID)
                    folderId = createFolder(folderReq.folder.info, infoUtente);
            }

            return folderId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static long createFolder(CorteContentServices.InfoNameType info, DocsPaVO.utente.InfoUtente infoUtente)
        {
            CorteContentServices.FolderCreateType folderCreate = new CorteContentServices.FolderCreateType();
            CorteContentServices.FolderCreateRequestType folderCreateReq = new CorteContentServices.FolderCreateRequestType();

            CorteContentServices.ItemIdResponseType itemResp;
            folderCreate.info = new DocsPaDocumentale_OCS.CorteContentServices.InfoType();
            folderCreate.info.name = info.name;
            folderCreate.info.location = info.location;
            folderCreate.info.state = DocsPaDocumentale_OCS.CorteContentServices.StateType.Undefined;
            folderCreate.info.author = infoUtente.userId;
            //controllare il formato
            //PS: sarebbe meglio se il formato lo mettesse OCS quando non viene specificato
            folderCreate.info.creationDate = System.DateTime.Now;

            folderCreateReq.folder = folderCreate;
            folderCreateReq.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();

            FolderManagementSOAPHTTPBinding wsFolder = OCSServiceFactory.GetDocumentServiceInstance<FolderManagementSOAPHTTPBinding>();
            itemResp = wsFolder.CreateFolder(folderCreateReq);

            OCSUtils.throwExceptionIfInvalidResult(itemResp.result);

            return itemResp.itemId;
        }
    }
}
