using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net;
using System.Xml;
using System.IO;
using System.Data;
using DocsPaVO.documento;

namespace DocsPaDocumentale_SP.WsSPServices
{
    /// <summary>
    /// Iacozzilli Giordano 08/10/2012
    /// Classe di utilità statiche per il Put , Get e utilità 
    /// collegate all'uso del documentale Sharepoint2010 sotto Docspa.
    /// </summary>

    public static class CallerSP
    {
        /// <summary>
        /// Putfile chiamato per l'inserimento in SharePoint del documento.
        /// </summary>
        /// <param name="fileforcontenct">Oggetto dal quale prendo i byte del File</param>
        /// <param name="schedaDocBase">Scheda documento, usata per i parametri in SharePoint</param>
        /// <param name="_infoutente">Info Utente</param>
        /// <param name="fileRequest">Oggetto utile alla get del nome file e del Path</param>
        /// <param name="extention">Estensione del file da inserire nel documentale (es:doc)</param>
        /// <returns>True false a seconda dell'esito del PUT, non uso Try catch per far tornare l'errore originale</returns>
        public static bool PutFileInSP(ref DocsPaVO.documento.FileDocumento fileforcontenct,
                                        ref SchedaDocumento schedaDocBase, DocsPaVO.utente.InfoUtente _infoutente,
                                        ref DocsPaVO.documento.FileRequest fileRequest, string extention)
        {
            string sRemoteFileURL = null;
            string sSPURL = GetServerRoot();
            //Istanzio il service
            SPCopy.Copy sp2010VmCopyService2Put = new SPCopy.Copy();
            //Setto le credenziali con le mie prese da db
            sp2010VmCopyService2Put.Credentials = GetCredentialSP();
            //eeeeeee
            //sp2010VmCopyService2Put.Url = string.Format("{0}/{1}", GetServerProtocollo() + GetServerRootnosites(), "_vti_bin/copy.asmx");
            sp2010VmCopyService2Put.Url =
            string.Format("{0}{1}/{2}/{3}", GetServerProtocollo(),GetServerRootnosites(),GetLibraryRoot(DateTime.Now.Year.ToString(), _infoutente.codWorkingApplication).Replace(@"/", "\\").ToUpper(), "_vti_bin/copy.asmx");
            
            //Nome file
            sRemoteFileURL = string.Format("{0}_{1}.{2}", _infoutente.codWorkingApplication, fileRequest.versionId, extention);
            //Url di destinazione sotto SP per la scrittura del file
            string[] destinationUrls = { Uri.EscapeUriString(GetServerProtocollo() + fileRequest.path) };
            //Faccio la Get della UO da passare nei Metadati
            DocsPaDB.Query_DocsPAWS.Utenti utentiDb = new DocsPaDB.Query_DocsPAWS.Utenti();
            DocsPaVO.utente.Ruolo ruolo = utentiDb.GetRuolo(_infoutente.idCorrGlobali, false);
            schedaDocBase.creatoreDocumento.uo_codiceCorrGlobali = ruolo.uo.codice;
            //Imposto i metadati.
            //correzione sabrina -- tutto l'oggetto fileRequest
            SPCopy.FieldInformation[] myFieldInfoArray = ImpostaMetaDati(ref schedaDocBase, _infoutente, fileRequest);
      
            //SPCopy.FieldInformation[] myFieldInfoArray = ImpostaMetaDati(ref schedaDocBase, _infoutente, fileRequest.version);
            SPCopy.CopyResult[] result;
            //Put del file sotto Sharepoint e sotto la sitecollection corretta
            sp2010VmCopyService2Put.CopyIntoItems(sRemoteFileURL, destinationUrls, myFieldInfoArray, fileforcontenct.content, out result);
            //Gestione errori:
            if (result[0].ErrorCode != SPCopy.CopyErrorCode.Success)
            {
                Console.WriteLine("Error occured during document upload process.");
                throw new Exception("Error Occured!" + result[0].ErrorCode);
            }

            //metadati null
            SPCopy.FieldInformation myFieldInfo = new SPCopy.FieldInformation();
            SPCopy.FieldInformation[] myFieldInfoArrayNull = { myFieldInfo };
            byte[] myByteArray;
            //Faccio la Get solo ed esclusivamente per valorizzare il nuovo contenct[] e il suo lenght.
            uint myGetUint = sp2010VmCopyService2Put.GetItem(destinationUrls[0], out myFieldInfoArrayNull, out myByteArray);
            //valorizzo.
            if (myByteArray == null || myByteArray.Length == 0)
                return false;
            fileforcontenct.content = myByteArray;
            fileforcontenct.length = myByteArray.Length;
            //Ok torno.
            return true;
        }

        /// <summary>
        /// Getfile chiamato per il Get da SharePoint del documento.
        /// </summary>
        /// <param name="filename">Nome del file da concatenare alla URL del server.</param>
        /// <param name="sitecollectionurl">Parametro che conterrà il nome della site collection (es: 2012_DOCSPA)</param>
        /// <returns>Il file richiesto a Sharepoint</returns>
        public static byte[] GetFileFromSP(string filename, string sitecollectionurl)
        {
            //Get root
            string sSPURL = GetServerRoot();
            //istanzio il service
            SPCopy.Copy sp2010VmCopyService2Get = new SPCopy.Copy();
            //Do le credenziali
            sp2010VmCopyService2Get.Credentials = GetCredentialSP();
            //Istanzio i metadato vuoti
            SPCopy.FieldInformation myFieldInfo = new SPCopy.FieldInformation();
            SPCopy.FieldInformation[] myFieldInfoArray = { myFieldInfo };
            byte[] myByteArray;
            //Get del documento da Sharepoint
            sp2010VmCopyService2Get.Url = string.Format("{0}/{1}", GetServerProtocollo() + sSPURL, sitecollectionurl + "/_vti_bin/copy.asmx");
            uint myGetUint = sp2010VmCopyService2Get.GetItem(GetServerProtocollo() + filename, out myFieldInfoArray, out myByteArray);
            //ritorno file
            return myByteArray;
        }

        ///// <summary>
        ///// Imposta i metadati da associare al documento in SharePoint, e sono:
        ///// DocNumber,segnatura,VersionId,AOO,UO,Utente,IdentificativoDocPrincipale.
        ///// </summary>
        ///// <param name="schedaDocPerMetadati">Scheda documeto passata già in Put</param>
        ///// <param name="_infoutente">Info Utente per reperire dati anagrafici (Utente, UO ecc)</param>
        ///// <param name="version">Versione del Documento</param>
        ///// <returns>FieldInformation[] da passare al server in fase di put.</returns>
        //private static SPCopy.FieldInformation[] ImpostaMetaDati(ref SchedaDocumento schedaDocPerMetadati,
        //                                                            DocsPaVO.utente.InfoUtente _infoutente, string version)
        //{
        //    // DocNumber (utilizzato per l’identificazione del documento);
        //    SPCopy.FieldInformation infoDocNumber = new SPCopy.FieldInformation();
        //    infoDocNumber.DisplayName = "DocNumber";
        //    infoDocNumber.InternalName = "DocNumber";
        //    infoDocNumber.Type = SPCopy.FieldType.Text;
        //    infoDocNumber.Value = (schedaDocPerMetadati.docNumber == null) ? "" : schedaDocPerMetadati.docNumber;

        //    //- segnatura (per l’identificazione del documento come protocollo; tale attributo sarà valorizzato unicamente per i documenti principali);
        //    SPCopy.FieldInformation infoSegnatura = new SPCopy.FieldInformation();
        //    infoSegnatura.DisplayName = "Segnatura";
        //    infoSegnatura.InternalName = "Segnatura";
        //    infoSegnatura.Type = SPCopy.FieldType.Text;
        //    infoSegnatura.Value = (schedaDocPerMetadati.protocollo == null) ? "" : schedaDocPerMetadati.protocollo.segnatura;

        //    //- VersionId (utilizzato per l’identificazione univoca di una specifica versione del documento);
        //    SPCopy.FieldInformation infoVersionId = new SPCopy.FieldInformation();
        //    infoVersionId.DisplayName = "VersionId";
        //    infoVersionId.InternalName = "VersionId";
        //    infoVersionId.Type = SPCopy.FieldType.Text;
        //    infoVersionId.Value = (version == null) ? "" : version;

        //    //- AOO (Area Organizzativa Omogenea),
        //    SPCopy.FieldInformation infoAOO = new SPCopy.FieldInformation();
        //    infoAOO.DisplayName = "AOO";
        //    infoAOO.InternalName = "AOO";
        //    infoAOO.Type = SPCopy.FieldType.Text;
        //    infoAOO.Value = (schedaDocPerMetadati.registro == null) ? "" : schedaDocPerMetadati.registro.codRegistro;

        //    //- UO (Unità Organizzativa),
        //    SPCopy.FieldInformation infoUO = new SPCopy.FieldInformation();
        //    infoUO.DisplayName = "UO";
        //    infoUO.InternalName = "UO";
        //    infoUO.Type = SPCopy.FieldType.Text;
        //    infoUO.Value = (schedaDocPerMetadati.creatoreDocumento == null) ? "" : schedaDocPerMetadati.creatoreDocumento.uo_codiceCorrGlobali;
        //    //- Utente (SP_Username creatore documento)
        //    SPCopy.FieldInformation infoUtente = new SPCopy.FieldInformation();
        //    infoUtente.DisplayName = "Utente";
        //    infoUtente.InternalName = "Utente";
        //    infoUtente.Type = SPCopy.FieldType.Text;
        //    infoUtente.Value = (_infoutente.userId == null) ? "" : _infoutente.userId;

        //    SPCopy.FieldInformation infoIdentificativoDocPrincipale = new SPCopy.FieldInformation();
        //    if ((schedaDocPerMetadati.documentoPrincipale != null) && schedaDocPerMetadati.documentoPrincipale.docNumber != "")
        //    {
        //        //- IdentificativoDocPrincipale (valorizzato unicamente per gli allegati al fine di avere anche su 
        //        // SharePoint evidenza del fatto che un documento ha senso di esistere solo in relazione ad altri documenti).
        //        infoIdentificativoDocPrincipale.DisplayName = "IdentificativoDocumentoPrincipale";
        //        infoIdentificativoDocPrincipale.InternalName = "IdentificativoDocumentoPrincipale";
        //        infoIdentificativoDocPrincipale.Type = SPCopy.FieldType.Text;
        //        infoIdentificativoDocPrincipale.Value = schedaDocPerMetadati.documentoPrincipale.docNumber;
        //    }

        //    SPCopy.FieldInformation[] containerMeta = { infoDocNumber, infoVersionId, infoSegnatura, infoAOO, infoUO, infoUtente, infoIdentificativoDocPrincipale };

        //    return containerMeta;
        //}



        /// <summary>
        /// Imposta i metadati da associare al documento in SharePoint, e sono:
        /// DocNumber,segnatura,VersionId,VersionLabel,AOO,UO,Utente,IdentificativoDocPrincipale.
        /// </summary>
        /// <param name="schedaDocPerMetadati">Scheda documeto passata già in Put</param>
        /// <param name="_infoutente">Info Utente per reperire dati anagrafici (Utente, UO ecc)</param>
        /// <param name="fileReq">Versione del Documento</param>
        /// <returns>FieldInformation[] da passare al server in fase di put.</returns>
        private static SPCopy.FieldInformation[] ImpostaMetaDati(ref SchedaDocumento schedaDocPerMetadati,
                                                                    DocsPaVO.utente.InfoUtente _infoutente, FileRequest fileReq)
        {
            // DocNumber (utilizzato per l’identificazione del documento);
            SPCopy.FieldInformation infoDocNumber = new SPCopy.FieldInformation();
            infoDocNumber.DisplayName = "DocNumber";
            infoDocNumber.InternalName = "DocNumber";
            infoDocNumber.Type = SPCopy.FieldType.Text;
            infoDocNumber.Value = (schedaDocPerMetadati.docNumber == null) ? "" : schedaDocPerMetadati.docNumber;

            //- segnatura (per l’identificazione del documento come protocollo; tale attributo sarà valorizzato unicamente per i documenti principali);
            SPCopy.FieldInformation infoSegnatura = new SPCopy.FieldInformation();
            infoSegnatura.DisplayName = "Segnatura";
            infoSegnatura.InternalName = "Segnatura";
            infoSegnatura.Type = SPCopy.FieldType.Text;
            infoSegnatura.Value = (schedaDocPerMetadati.protocollo == null) ? "" : schedaDocPerMetadati.protocollo.segnatura;

            //- VersionId (utilizzato per l’identificazione univoca di una specifica versione del documento);
            SPCopy.FieldInformation infoVersionId = new SPCopy.FieldInformation();
            infoVersionId.DisplayName = "VersionId";
            infoVersionId.InternalName = "VersionId";
            infoVersionId.Type = SPCopy.FieldType.Text;
            infoVersionId.Value = (fileReq.versionId == null) ? "" : fileReq.versionId;

            //sabrina
            //- VersionLabel (utilizzato per l’identificazione univoca di una specifica versione del documento);
            SPCopy.FieldInformation infoVersionLabel = new SPCopy.FieldInformation();
            infoVersionLabel.DisplayName = "VersionLabel";
            infoVersionLabel.InternalName = "VersionLabel";
            infoVersionLabel.Type = SPCopy.FieldType.Text;
            infoVersionLabel.Value = (fileReq.versionLabel == null) ? "" : fileReq.versionLabel;

            //- AOO (Area Organizzativa Omogenea),
            SPCopy.FieldInformation infoAOO = new SPCopy.FieldInformation();
            infoAOO.DisplayName = "AOO";
            infoAOO.InternalName = "AOO";
            infoAOO.Type = SPCopy.FieldType.Text;
            infoAOO.Value = (schedaDocPerMetadati.registro == null) ? "" : schedaDocPerMetadati.registro.codRegistro;

            //- UO (Unità Organizzativa),
            SPCopy.FieldInformation infoUO = new SPCopy.FieldInformation();
            infoUO.DisplayName = "UO";
            infoUO.InternalName = "UO";
            infoUO.Type = SPCopy.FieldType.Text;
            infoUO.Value = (schedaDocPerMetadati.creatoreDocumento == null) ? "" : schedaDocPerMetadati.creatoreDocumento.uo_codiceCorrGlobali;
            //- Utente (SP_Username creatore documento)
            SPCopy.FieldInformation infoUtente = new SPCopy.FieldInformation();
            infoUtente.DisplayName = "Utente";
            infoUtente.InternalName = "Utente";
            infoUtente.Type = SPCopy.FieldType.Text;
            infoUtente.Value = (_infoutente.userId == null) ? "" : _infoutente.userId;

            SPCopy.FieldInformation infoIdentificativoDocPrincipale = new SPCopy.FieldInformation();
            if ((schedaDocPerMetadati.documentoPrincipale != null) && schedaDocPerMetadati.documentoPrincipale.docNumber != "")
            {
                //- IdentificativoDocPrincipale (valorizzato unicamente per gli allegati al fine di avere anche su 
                // SharePoint evidenza del fatto che un documento ha senso di esistere solo in relazione ad altri documenti).
                infoIdentificativoDocPrincipale.DisplayName = "IdentificativoDocumentoPrincipale";
                infoIdentificativoDocPrincipale.InternalName = "IdentificativoDocumentoPrincipale";
                infoIdentificativoDocPrincipale.Type = SPCopy.FieldType.Text;
                infoIdentificativoDocPrincipale.Value = schedaDocPerMetadati.documentoPrincipale.docNumber;
            }

            SPCopy.FieldInformation[] containerMeta = { infoDocNumber, infoVersionId, infoVersionLabel, infoSegnatura, infoAOO, infoUO, infoUtente, infoIdentificativoDocPrincipale };

            return containerMeta;
        }




        /// <summary>
        /// Get delle credenziali SharePoint dal Database.
        /// sSP_User,sSP_Pwd,sDomain
        /// </summary>
        /// <returns></returns>
        private static ICredentials GetCredentialSP()
        {
            string sSP_User = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "SP_USER");
            string sSP_Pwd = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "SP_PWD");
            string sDomain = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "SP_DOMAIN");

            ICredentials _retCred = new System.Net.NetworkCredential(sSP_User, sSP_Pwd, sDomain);
            return _retCred;
        }

        /// <summary>
        /// Get Nome site collection da database:
        /// SP_SiteCollection
        /// </summary>
        /// <param name="anno">Anno protocollazione</param>
        /// <param name="app_chiamante">App chiamante (docspa, altre)</param>
        /// <returns></returns>
        public static string GetLibraryRoot(string anno, string app_chiamante)
        {
            string sAppName = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "SP_SITECOLLECTION");
            return sAppName.Replace("ANNO", anno).Replace("APPLICATIVO", app_chiamante);
        }

        /// <summary>
        /// Get Servername dal database:
        /// SP_Server
        /// </summary>
        /// <returns></returns>
        public static string GetServerRoot()
        {
            string sSPURL = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "SP_SERVER");
            return sSPURL;
        }

        /// <summary>
        /// Get Servername dal database senza Sites:
        /// SP_Server
        /// </summary>
        /// <returns></returns>
        public static string GetServerRootnosites()
        {
            string sSPURL = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "SP_SERVER");
            return sSPURL.Split('/')[0];
        }

        /// <summary>
        /// Get del tipo di Protocollo:
        /// http://, https://, ecc..
        /// </summary>
        /// <returns></returns>
        public static string GetServerProtocollo()
        {
            string sProt = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "SP_PROTOCOLLO");
            return sProt;
        }

        /// <summary>
        /// Get del path completo assoluto del documento
        /// </summary>
        /// <param name="anno"></param>
        /// <param name="app_chiamante"></param>
        /// <returns></returns>
        public static string GetPathAbsolute(string anno, string app_chiamante)
        {

            string sSPURL = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "SP_SERVER");
            string sAppName = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "SP_SITECOLLECTION");
            string sDocLib = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "SP_DOCUMENTLIBRARY");

            //Faccio la get del divisore per dividere le site collection
            string _divide = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "SP_SITECOLLECTIONTIMEDIVIDE");

            DateTime sDeathDate = DateTime.Now;
            //Trovo in quale parte dell'anno siamo a seconda del divisore.
            int _result = (int)Math.Ceiling(sDeathDate.Month / Convert.ToDecimal(_divide));
            //Se il divisore è 1 lascio tutto comè:
            if (_divide == "1")
                return (sSPURL + "/" + sAppName.Replace("ANNO", anno).Replace("APPLICATIVO", app_chiamante) +
                        "/" + sDocLib + "/");
            else
                return (sSPURL + "/" + sAppName.Replace("ANNO", anno + "_" + _divide + "_" + _result).Replace("APPLICATIVO", app_chiamante) +
                         "/" + sDocLib + "/");
        }

        /// <summary>
        /// Get dell'URI del servizio Sharepoin di copy.
        /// </summary>
        /// <returns></returns>
        private static string GetSPServiceUrl()
        {
            string sSPURL = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "SP_SERVER");
            return sSPURL + "/_vti_bin/copy.asmx";
        }
    }

}
