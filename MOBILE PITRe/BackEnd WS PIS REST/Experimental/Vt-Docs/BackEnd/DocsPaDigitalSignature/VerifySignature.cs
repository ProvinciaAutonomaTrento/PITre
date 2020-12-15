using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Configuration;
using DocsPaUtils.LogsManagement;
using DocsPaUtils.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Pkcs;
using log4net;
using System.Collections.Generic;
using DocsPaVO.SmartClient;
using DocsPaUtils.ConverterDate;

namespace BusinessLogic.Documenti.DigitalSignature
{
    /// <summary>
    /// 
    /// </summary>
    public class VerifySignature
    {
        private ILog logger = LogManager.GetLogger(typeof(VerifySignature));

        public VerifySignature()
        {
        }

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathName"></param>
        /// <returns></returns>
        public DocsPaVO.documento.VerifySignatureResult Verify(string pathName)
        {
            return Verify(pathName);
        }
        */
        /// <summary>
        /// Controllo Esterno per gli M7M
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="dataVerifica"></param>
        /// <returns></returns>
        public DocsPaVO.documento.VerifySignatureResult VerifyM7M(string pathName)
        {
            DocsPaVO.documento.VerifySignatureResult vsr = new DocsPaVO.documento.VerifySignatureResult();
            try
            {
                string CRLServiceUrl = CLRVerificationServiceUrl();
                string file = Path.Combine(this.GetPKCS7InputDirectory(), pathName);

                ExternalModule ext = new ExternalModule();
                bool result = false;
                if (CRLServiceUrl != null)
                    result = ext.externalClrCheckWS(CRLServiceUrl, file, DateTime.MinValue);
                else
                    return null;

                if (!result)
                {
                    string errDesc = String.Format("Il modulo di verifica esterno ha dato esito negativo: (ceritificato scaduto/revocato/errore)  argomento {0} file {1}", CRLServiceUrl, file);
                    if (CRLServiceUrl == null)
                        errDesc += " controllate l'argomento del modulo di verifica che è null o vuoto";

                    logger.Debug(errDesc);

                    if (ext.Esito.message != string.Empty)
                        logger.Error(ext.Esito.message);

                }
                vsr = ext.Esito.VerifySignatureResult;
            }
            catch (Exception e)
            {
                vsr.StatusCode = -1;
                vsr.StatusDescription = e.Message;
                logger.Debug("eccezione in verify External: " + e.Message);
            }
            return vsr;
        }

        public Dictionary<string, DocsPaVO.documento.SignerInfo> getSignerInfoFromExternalVerify(DocsPaVO.documento.VerifySignatureResult vsr)
        {
            Dictionary<string, DocsPaVO.documento.SignerInfo> siList = new Dictionary<string, DocsPaVO.documento.SignerInfo>();
            foreach (DocsPaVO.documento.PKCS7Document p7d in vsr.PKCS7Documents)
            {
                if (p7d.SignersInfo != null)
                {
                    foreach (DocsPaVO.documento.SignerInfo si in p7d.SignersInfo)
                    {
                        if (!string.IsNullOrEmpty(si.SubjectInfo.CertId))
                        {
                            string siIndex = string.Format("{0}§{1}", si.SubjectInfo.CertId.ToUpper(), si.SubjectInfo.CommonName.ToUpper());
                            if (!siList.ContainsKey(siIndex))
                                siList.Add(siIndex, si);
                        }
                    }
                }
            }
            return siList;
        }

        public DocsPaVO.documento.VerifySignatureResult setSignerInfoFromExternalVerify(Dictionary<string, DocsPaVO.documento.SignerInfo> siList, EsitoVerifica clrExternalStatus, DocsPaVO.documento.VerifySignatureResult vsr)
        {
            for (int x = 0; x < vsr.PKCS7Documents.Length; x++)
            {
                DocsPaVO.documento.PKCS7Document p7doc = vsr.PKCS7Documents[x];
                //foreach (DocsPaVO.documento.SignerInfo si in p7doc.SignersInfo)  //la foreach nun ja po fà...
                for (int i = 0; i < p7doc.SignersInfo.Length; i++)
                {
                    DocsPaVO.documento.SignerInfo si = p7doc.SignersInfo[i];
                    //Questo codice integra il check Esterno 
                    vsr.CRLOnlineCheck = true;
                    string siIndex = string.Format("{0}§{1}", si.SubjectInfo.CertId.ToUpper(), si.SubjectInfo.CommonName.ToUpper());
                    if (siList.ContainsKey(siIndex))
                    {
                        DocsPaVO.documento.SignerInfo siTmp = siList[siIndex];
                        si.CertificateInfo.RevocationStatusDescription = string.Empty;
                        if (siTmp.CertificateInfo.RevocationDate != DateTime.MinValue)
                        {
                            si.CertificateInfo.RevocationDate = siTmp.CertificateInfo.RevocationDate;
                            si.CertificateInfo.RevocationStatusDescription = String.Format("Il {0} ", siTmp.CertificateInfo.RevocationDate.ToShortDateString());
                        }
                        si.CertificateInfo.RevocationStatusDescription += new SignedDocument().DecodeStatus(siTmp.CertificateInfo.RevocationStatus);

                        //popoliamo la revocation status SOLO se diverso da valid
                        if ((clrExternalStatus.status != EsitoVerificaStatus.Valid))
                            si.CertificateInfo.RevocationStatus = siTmp.CertificateInfo.RevocationStatus;
                        else
                            si.CertificateInfo.RevocationStatus = 0;

                        p7doc.SignersInfo[i] = si;
                    }
                }
            }
            return vsr;
        }

        public DocsPaVO.documento.VerifySignatureResult Verify(string pathName)
        {
            bool crlOnlineCheck = (this.CRLOnlineCheckEnabled());
            return Verify(pathName, crlOnlineCheck);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="dataVerifica"></param>
        /// <returns></returns>
        public DocsPaVO.documento.VerifySignatureResult Verify(string pathName, bool crlOnlineCheck)
        {
            int rc = 0;
            DocsPaVO.documento.VerifySignatureResult vsr = new DocsPaVO.documento.VerifySignatureResult();
            ArrayList alP7Docs = new ArrayList();
            List<string> ErrorMessageLst = new List<string>();
            SignedDocument mainP7Doc, previousP7Doc, currentP7Doc;
            int documentLevel = 0;

            vsr.CRLOnlineCheck = crlOnlineCheck;
            logger.Debug("crlOnlineCheck: " + crlOnlineCheck.ToString());
            try
            {
                // load PKCS7 document ...
                logger.Debug("load PKCS7 document");

                mainP7Doc = new SignedDocument(Path.Combine(this.GetPKCS7InputDirectory(), pathName));
                logger.Debug("loaded");

                if (mainP7Doc.Buffer != null)
                {
                    logger.Debug("buffr docpkcs7: " + mainP7Doc.Buffer.Length);
                    mainP7Doc.CRLOnlineCheck = crlOnlineCheck;
                    mainP7Doc.CRLRetrieveTimeout = this.GetCRLTimeout() * 1000;

                    bool retval = false;
                    try
                    {
                        retval = mainP7Doc.Verify(ref ErrorMessageLst);
                    }
                    catch (Exception e)
                    {
                        retval = false;
                        throw e;
                    }
                    if (retval) // verify signature 
                    {
                        logger.Debug("main verify ok  ");
                        // if verified OK, add document to array
                        documentLevel = alP7Docs.Add(mainP7Doc);
                        previousP7Doc = (SignedDocument)alP7Docs[documentLevel];
                        logger.Debug("main  ok  ");
                        // search for nested P7M docs and recursively verify ...
                        while (Path.GetExtension(previousP7Doc.DocumentFileName).ToLower() == ".p7m")
                        {
                            logger.Debug("main while start  ");
                            currentP7Doc = new SignedDocument(previousP7Doc.Content, previousP7Doc.DocumentFileName);
                            currentP7Doc.CRLRetrieveTimeout = this.GetCRLTimeout() * 1000;
                            currentP7Doc.CRLOnlineCheck = crlOnlineCheck;
                            if (currentP7Doc.Verify(ref ErrorMessageLst))
                            {
                                logger.Debug("verify p7doc ok  ");
                                currentP7Doc.Level = documentLevel + 1;
                                documentLevel = alP7Docs.Add(currentP7Doc);
                                previousP7Doc = currentP7Doc;
                            }
                            else
                            {
                                logger.Debug("verify p7doc ok  ");
                                break;
                            }
                        }
                        // convert arraylist to simple structure array ...
                        vsr.PKCS7Documents = this.CreatePKCS7DocumentArray(alP7Docs);
                        vsr.FinalDocumentName = previousP7Doc.DocumentFileName;
                        // save final document in 'Output' folder ...
                        logger.Debug("verify p7doc  write to output folder ");
                        previousP7Doc.SaveContentToFile(Path.Combine(this.GetPKCS7OutputDirectory(), previousP7Doc.DocumentFileName));
                        logger.Debug("verify p7doc ok write to output folder ");
                    }

                    // Evaluate global status code (search for any revocation status != 0) ...
                    //foreach (DocsPaVO.documento.PKCS7Document p7doc in vsr.PKCS7Documents)
                    
                    

                    for (int x = 0; x < vsr.PKCS7Documents.Length; x++)
                    {
                        DocsPaVO.documento.PKCS7Document p7doc = vsr.PKCS7Documents[x];
                        //foreach (DocsPaVO.documento.SignerInfo si in p7doc.SignersInfo)  //la foreach nun ja po fà...
                        for (int i = 0; i < p7doc.SignersInfo.Length; i++)
                        {
                            DocsPaVO.documento.SignerInfo si = p7doc.SignersInfo[i];
                            //Questo codice integra il check Esterno 

                            vsr.PKCS7Documents[x].SignersInfo[i] = si;

                            if (si.SignatureAlgorithm.ToUpperInvariant().Replace("-","").Contains("SHA1"))
                                ErrorMessageLst.Add("Firmato SHA-1");

                            if (si.CertificateInfo.RevocationStatus != 0)
                            {
                                rc = -1;
                                break;
                            }
                            vsr.PKCS7Documents[x] = p7doc;
                        }
                    }
                    vsr.StatusCode = rc;
                    vsr.StatusDescription = mainP7Doc.DecodeStatus(rc);
                }
            }
            catch (Exception e)
            {
                vsr.StatusCode = -1;
                vsr.StatusDescription = e.Message;
                logger.Debug("eccezione in verify: " + e.Message);
            }


            if (ErrorMessageLst.Count > 0)
            {
                vsr.ErrorMessages = ErrorMessageLst.ToArray();
                if (ErrorMessageLst.Contains("Id-AA-SigningCertificateV2 not found") ||
                    ErrorMessageLst.Contains("Missing SignedAttributes"))
                {
                    vsr.StatusCode = -5;
                    vsr.StatusDescription = "IdAASigningCertificateV2 not found";
                }
                else
                {
                    vsr.StatusCode = -1;//generico
                }
            }
            return vsr;
        }

        public DocsPaVO.documento.CertificateInfo VerifyCertificate_External(DocsPaVO.documento.CertificateInfo certificate)
        {
            string CRLServiceUrl = CLRVerificationServiceUrl();
            
            ExternalModule ext = new ExternalModule();
            

            EsitoVerifica esito = null;
            if (CRLServiceUrl != null)
                esito = ext.externalCertCheckWs (CRLServiceUrl, certificate);
            else
                return certificate;
            logger.Debug("VERIFY_SIGNATURE: ERROR_CODE " + esito.errorCode);
            logger.Debug("VERIFY_SIGNATURE: MSG_CODE " + esito.message);
            DocsPaVO.documento.CertificateInfo cinfo = new DocsPaVO.documento.CertificateInfo();
            try
            {
                cinfo = ext.Esito.VerifySignatureResult.PKCS7Documents[0].SignersInfo[0].CertificateInfo;
                if (!string.IsNullOrEmpty (esito.errorCode))
                {
                    
                    Int32.TryParse (esito.errorCode,out  cinfo.RevocationStatus);
                    foreach (string s in esito.additionalData)
                        cinfo.RevocationStatusDescription = s;
                }
                
            }
            catch (Exception e)
            {
                //In caso d'errore (non posso raggiungere il server)   facciamo tornare -500
                //if (!result)
                    cinfo.RevocationStatus = -500;
                return cinfo;
            }
            return cinfo;
        }

        public DocsPaVO.documento.CertificateInfo VerifyCertificateExpired(DocsPaVO.documento.CertificateInfo certificate)
        {
            string CRLServiceUrl = CLRVerificationServiceUrl();

            ExternalModule ext = new ExternalModule();
            logger.Debug("Inizio VerifyCertificateExpired");

            EsitoVerifica esito = null;
            string key = "BE_CHECK_CRLCLIENT";
            string idAmm = "0";
            if (CRLServiceUrl == null || 
                (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, key)) && DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, key) == "1"))
            {
                return VerifyCertificateDateExpired(certificate);
            }
            

            DocsPaVO.documento.CertificateInfo cinfo = new DocsPaVO.documento.CertificateInfo();
            try
            {
                esito = ext.externalCertCheckWs(CRLServiceUrl, certificate);
                logger.Debug("VERIFY_SIGNATURE: ERROR_CODE " + esito.errorCode);
                logger.Debug("VERIFY_SIGNATURE: MSG_CODE " + esito.message);
                cinfo = ext.Esito.VerifySignatureResult.PKCS7Documents[0].SignersInfo[0].CertificateInfo;
                if (!string.IsNullOrEmpty(esito.errorCode))
                {

                    Int32.TryParse(esito.errorCode, out  cinfo.RevocationStatus);
                    foreach (string s in esito.additionalData)
                        cinfo.RevocationStatusDescription = s;
                }

            }
            catch (Exception e)
            {
                //In caso d'errore (non posso raggiungere il server)   controllo 
                //if (!result)
                cinfo = VerifyCertificateDateExpired(certificate);
            }

            logger.Debug("Fine VerifyCertificateExpired");

            return cinfo;
        }

        public DocsPaVO.documento.CertificateInfo VerifyCertificateDateExpired(DocsPaVO.documento.CertificateInfo certificate)
        {
            int verificaStatus = (int)EsitoVerificaStatus.Valid;
            DateTime dateTimeNow = DateTime.Now;
            DocsPaDB.Query_Utils.Utils utils = new DocsPaDB.Query_Utils.Utils();
            string dateTimeStr = string.Empty;
            int compareResult = 0;
            IConverterDate converterDate = null;
            try
            {
                logger.Debug("VERIFY_CERTIFICATE_DATE INIZIO");
                dateTimeNow = utils.SelectDBDateTime();
                converterDate = new ConvertDateComponent();

                if (String.IsNullOrEmpty(certificate.ValidFromDateStr))
                    throw new Exception("ValidFromDateStr NOT FOUND");
                if (String.IsNullOrEmpty(certificate.ValidToDateStr))
                    throw new Exception("ValidToDateStr NOT FOUND");

                logger.Debug("VERIFY_CERTIFICATE_DATE certificate.ValidFromDate : " + certificate.ValidFromDateStr);
                logger.Debug("VERIFY_CERTIFICATE_DATE certificate.ValidToDate : " + certificate.ValidToDateStr);

                certificate.ValidFromDate = converterDate.getDateTime(certificate.ValidFromDateStr);
                certificate.ValidToDate = converterDate.getDateTime(certificate.ValidToDateStr);

                compareResult = DateTime.Compare(certificate.ValidFromDate, dateTimeNow);
                if (compareResult > 0)
                    verificaStatus = (int)EsitoVerificaStatus.NotTimeValid;
                else
                    verificaStatus = (int)EsitoVerificaStatus.Valid;

                compareResult = DateTime.Compare(certificate.ValidToDate, dateTimeNow);
                if (compareResult < 0)
                    verificaStatus = (int)EsitoVerificaStatus.NotTimeValid;
                else
                    verificaStatus = (int)EsitoVerificaStatus.Valid;

                certificate.RevocationStatus = verificaStatus;
                logger.Debug("VERIFY_CERTIFICATE_DATE FINE STATUS " + certificate.RevocationStatus);
            }
            catch (Exception e)
            {
                logger.Error("VERIFY_CERTIFICATE_DATE: EXCEPTION: "+e.Message);
                verificaStatus = (int)EsitoVerificaStatus.ErroreGenerico;
                certificate.RevocationStatus = verificaStatus;
            }
            return certificate;
        }


        public DocsPaVO.documento.VerifySignatureResult Verify_External(DocsPaVO.documento.FileDocumento  fileDoc , DateTime dataVerifica)
        {
            string pathName = fileDoc.name;
            string backName = fileDoc.name;
            logger.Debug("Leggo il file input per backup");
            byte[] backup = File.ReadAllBytes(Path.Combine(this.GetPKCS7InputDirectory(), pathName));
            DocsPaVO.documento.VerifySignatureResult vsr = null;
            //la nostra verifca vuole un file p7m non un tsd o un m7m, lo devo prima sbustare nel caso fosse "imbustato"
            DocsPaVO.documento.TsType type = Helpers.getFileType(backup);
            if ((type != DocsPaVO.documento.TsType.PKCS && type != DocsPaVO.documento.TsType.UNKNOWN)
                || (type == DocsPaVO.documento.TsType.UNKNOWN && !Path.GetExtension(fileDoc.name).ToUpper().Equals(".P7M")))
            {
                //il nome non deve portare l'estensione tsd o m7m, perchè la verifica interna è sensibile al contensto p7m
                string fileName_notsd = Path.Combine ( this.GetPKCS7InputDirectory() , Path.GetFileNameWithoutExtension(fileDoc.name));
                logger.Debug("Scrivo il file sbustato dal suo timestamp per verifica interna");
                File.WriteAllBytes(fileName_notsd, Helpers.sbustaFileTimstamped(backup));
                vsr = Verify(fileName_notsd, false);
                
                try
                {
                    File.Delete(fileName_notsd);
                }
                catch { };
                //tolgo un estensione al filedoc.name
                fileDoc.name =  Path.GetFileNameWithoutExtension(backName);
            }
            else
            {
                //la verify sbusta il file p7m e lo risalva, ma a zeni devo mandare il file originale, quindi riscrivo il file letto poco sopra.
                vsr = Verify(pathName, false);

                DocsPaVO.documento.FileDocumento fd1 = fileDoc ;
                fd1.content =Helpers.sbustaFileFirmato(fileDoc.content);
                fd1.signatureResult = vsr;
                if (Pades_Utils.Pades.IsPdfPades(fd1))
                {
                    Pades_Utils.Pades.VerifyPadesSignature(fd1);
                    vsr = fd1.signatureResult;
                }

            }
            //File.WriteAllBytes (Path.Combine(this.GetPKCS7InputDirectory(), pathName),backup);
            if (Helpers.getFileType(backup) != DocsPaVO.documento.TsType.PKCS)
            {
                logger.Debug("Scrivo il file sbustato dal suo timestamp per verifica esterna");
                File.WriteAllBytes(Path.Combine(this.GetPKCS7InputDirectory(), pathName), Helpers.sbustaFileTimstamped(backup));
            }


            //prima del 1/7/2011 gli sha1 sono ok, se presenti dalla verify (che non controlla le date), li rimuovo.
            DateTime SHA1NonValido = DateTime.Parse("2011-06-30 23:59:59");
            if (dataVerifica < SHA1NonValido)
            {
                List<string> ers = new List<string>();
                foreach (string s in vsr.ErrorMessages)
                {
                    if (s.Equals("Firmato SHA-1"))
                        continue;

                    ers.Add(s);
                }
                if (ers.Count > 0)
                    vsr.ErrorMessages = ers.ToArray();
            }

            //crepo la variaible backup.
            backup = null;

            ArrayList alP7Docs = new ArrayList();
            List<string> ErrorMessageLst = new List<string>();

            #region Verifica Esterna
            //Dati CertEsterno
            try
            {
                string CRLServiceUrl = CLRVerificationServiceUrl();
                string file = Path.Combine(this.GetPKCS7InputDirectory(), pathName);

                ExternalModule ext = new ExternalModule();
                bool result = false;
                if (CRLServiceUrl != null)
                    result = ext.externalClrCheckWS(CRLServiceUrl, file, dataVerifica);
                else
                    return null;

                //per i pades
                DocsPaVO.documento.VerifySignatureResult padesRetval = PadesExternalCheck(pathName, file, ext);
                if (padesRetval != null)
                    return padesRetval;

                if (!result)
                {
                    string errDesc = String.Format("Il modulo di verifica esterno ha dato esito negativo: (ceritificato scaduto/revocato/errore) CRLServiceUrl {0} file {1}", CRLServiceUrl, file);
                    if (CRLServiceUrl == null)
                        errDesc += " controllate l'argomento del modulo di verifica che è null o vuoto";

                    logger.Debug(errDesc);

                    if (ext.Esito.message != string.Empty)
                    {
                        logger.Error(ext.Esito.message);
                        ErrorMessageLst.Add(ext.Esito.message);
                    }
                }
                if (ext.Esito != null)
                {
                    Dictionary<string, DocsPaVO.documento.SignerInfo> siList = getSignerInfoFromExternalVerify(ext.Esito.VerifySignatureResult);
                    //popolo la signerInfo con il valore di ritorno
                    vsr = setSignerInfoFromExternalVerify(siList, ext.Esito, vsr);

                    //e' stata fatta la verifica online (servizio esterno)
                    vsr.CRLOnlineCheck = true;

                    if (!result)
                    {
                        string errMsg = String.Format("EXTCHECK : {0}-{1}", ext.Esito.errorCode, ext.Esito.message);
                        ErrorMessageLst.Add(String.Format(errMsg));
                        ErrorMessageLst.Add(vsr.StatusDescription);
                        vsr.StatusDescription += " " + errMsg;

                        if (vsr.StatusCode == 0)
                            vsr.StatusCode = -1;
                    }
                    else
                    {
                        vsr.StatusDescription = ext.Esito.VerifySignatureResult.StatusDescription;
                        vsr.StatusCode = ext.Esito.VerifySignatureResult.StatusCode;
                    }
                }
            }
            catch (Exception e)
            {
                if (vsr.CRLOnlineCheck == false)
                    vsr.StatusCode = -100;  //problemi a raggiungere il server
                else
                    vsr.StatusCode = -1;

                vsr.StatusDescription = e.Message;
                ErrorMessageLst.Add(vsr.StatusDescription);
                logger.Debug("eccezione in verify External: " + e.Message);
            }

            #endregion

            vsr.ErrorMessages = ErrorMessageLst.ToArray();

            
            //strippa l'estensione
            vsr.FinalDocumentName = stripExtraExtensions(vsr.FinalDocumentName);

            return vsr;
        }

        private string stripExtraExtensions(string fileName)
        {
            string ext = "";
            while (!String.IsNullOrEmpty ( Path.GetExtension (fileName)))
            {
                ext = Path.GetExtension(fileName);
                fileName = Path.GetFileNameWithoutExtension(fileName);
            }
            return fileName + ext;
        }


    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="dataVerifica"></param>
        /// <returns></returns>
        public DocsPaVO.documento.VerifySignatureResult Verify_OLD_DEAD_CODE(string pathName, DateTime? dataVerifica)
        {
            int rc = 0;
            DocsPaVO.documento.VerifySignatureResult vsr = new DocsPaVO.documento.VerifySignatureResult();
            ArrayList alP7Docs = new ArrayList();
            List<string> ErrorMessageLst = new List<string>();
            SignedDocument mainP7Doc, previousP7Doc, currentP7Doc;
            int documentLevel = 0;
            bool crlOnlineCheck = (this.CRLOnlineCheckEnabled());

            #region Verifica Esterna
            //Dati CertEsterno
            EsitoVerifica clrExternalStatus = null;
            Dictionary<string, DocsPaVO.documento.SignerInfo> siList = new Dictionary<string, DocsPaVO.documento.SignerInfo>();
            if (dataVerifica != null)
            {
                crlOnlineCheck = false; //Disabilito il Check interno
                try
                {
                    string CRLServiceUrl = CLRVerificationServiceUrl();
                    string file = Path.Combine(this.GetPKCS7InputDirectory(), pathName);

                    ExternalModule ext = new ExternalModule();
                    bool result = false;
                    if (CRLServiceUrl != null)
                        result = ext.externalClrCheckWS(CRLServiceUrl, file, dataVerifica.Value);
                    else
                        return null;

                    if (!result)
                    {
                        string errDesc = String.Format("Il modulo di verifica esterno ha dato esito negativo: (ceritificato scaduto/revocato/errore) CRLServiceUrl {0} file {1}", CRLServiceUrl, file);
                        if (CRLServiceUrl == null)
                            errDesc += " controllate l'argomento del modulo di verifica che è null o vuoto";

                        logger.Debug(errDesc);

                        if (ext.Esito.message != string.Empty)
                            logger.Error(ext.Esito.message);

                    }

                    if (ext.Esito != null)
                        clrExternalStatus = ext.Esito;

                    if (ext.Esito.VerifySignatureResult.PKCS7Documents != null)
                    {
                        foreach (DocsPaVO.documento.PKCS7Document p7d in ext.Esito.VerifySignatureResult.PKCS7Documents)
                        {
                            if (p7d.SignersInfo != null)
                            {
                                foreach (DocsPaVO.documento.SignerInfo si in p7d.SignersInfo)
                                {
                                    string siIndex = string.Format("{0}§{1}", si.SubjectInfo.CertId.ToUpper(), si.SubjectInfo.CommonName.ToUpper());
                                    if (!siList.ContainsKey(siIndex))
                                        siList.Add(siIndex, si);
                                }
                            }
                        }
                    }

                    DocsPaVO.documento.VerifySignatureResult padesRetval = PadesExternalCheck(pathName, file, ext);
                    if (padesRetval != null)
                        return padesRetval;
                }
                catch (Exception e)
                {
                    vsr.StatusCode = -1;
                    vsr.StatusDescription = e.Message;
                    logger.Debug("eccezione in verify External: " + e.Message);
                }
            }
            #endregion

            #region InternalCheck
            vsr.CRLOnlineCheck = crlOnlineCheck;
            logger.Debug("crlOnlineCheck: " + crlOnlineCheck.ToString());
            try
            {
                // load PKCS7 document ...
                logger.Debug("load PKCS7 document");

                mainP7Doc = new SignedDocument(Path.Combine(this.GetPKCS7InputDirectory(), pathName));
                logger.Debug("loaded");


                if (mainP7Doc.Buffer != null)
                {
                    logger.Debug("buffr docpkcs7: " + mainP7Doc.Buffer.Length);
                    mainP7Doc.CRLRetrieveTimeout = this.GetCRLTimeout() * 1000;
                    mainP7Doc.CRLOnlineCheck = crlOnlineCheck;
                    bool retval = false;
                    try
                    {
                        retval = mainP7Doc.Verify(ref ErrorMessageLst);
                    }
                    catch (Exception e)
                    {
                        retval = false;
                        throw e;
                    }
                    if (retval) // verify signature 
                    {
                        logger.Debug("main verify ok  ");
                        // if verified OK, add document to array
                        documentLevel = alP7Docs.Add(mainP7Doc);
                        previousP7Doc = (SignedDocument)alP7Docs[documentLevel];
                        logger.Debug("main  ok  ");
                        // search for nested P7M docs and recursively verify ...
                        while (Path.GetExtension(previousP7Doc.DocumentFileName).ToLower() == ".p7m")
                        {
                            logger.Debug("main while start  ");
                            currentP7Doc = new SignedDocument(previousP7Doc.Content, previousP7Doc.DocumentFileName);
                            currentP7Doc.CRLRetrieveTimeout = this.GetCRLTimeout() * 1000;
                            currentP7Doc.CRLOnlineCheck = crlOnlineCheck;
                            if (currentP7Doc.Verify(ref ErrorMessageLst))
                            {
                                logger.Debug("verify p7doc ok  ");
                                currentP7Doc.Level = documentLevel + 1;
                                documentLevel = alP7Docs.Add(currentP7Doc);
                                previousP7Doc = currentP7Doc;
                            }
                            else
                            {
                                logger.Debug("verify p7doc ok  ");
                                break;
                            }
                        }
                        // convert arraylist to simple structure array ...
                        vsr.PKCS7Documents = this.CreatePKCS7DocumentArray(alP7Docs);
                        vsr.FinalDocumentName = previousP7Doc.DocumentFileName;
                        // save final document in 'Output' folder ...
                        logger.Debug("verify p7doc  write to output folder ");
                        previousP7Doc.SaveContentToFile(Path.Combine(this.GetPKCS7OutputDirectory(), previousP7Doc.DocumentFileName));
                        logger.Debug("verify p7doc ok write to output folder ");
                    }

                    // Evaluate global status code (search for any revocation status != 0) ...
                    //foreach (DocsPaVO.documento.PKCS7Document p7doc in vsr.PKCS7Documents)

                    bool ValidoNelleDate = false;
                    DateTime maxDate = DateTime.MinValue;
                    DateTime minDate = DateTime.MinValue;

                    for (int x = 0; x < vsr.PKCS7Documents.Length; x++)
                    {
                        DocsPaVO.documento.PKCS7Document p7doc = vsr.PKCS7Documents[x];
                        //foreach (DocsPaVO.documento.SignerInfo si in p7doc.SignersInfo)  //la foreach nun ja po fà...
                        for (int i = 0; i < p7doc.SignersInfo.Length; i++)
                        {
                            DocsPaVO.documento.SignerInfo si = p7doc.SignersInfo[i];
                            //Questo codice integra il check Esterno 

                            #region GetFromCheckEsterno
                            if (clrExternalStatus != null)
                            {
                                vsr.CRLOnlineCheck = true;
                                //if ((clrExternalStatus.status != EsitoVerificaStatus.Valid))
                                {
                                    string siIndex = string.Format("{0}§{1}", si.SubjectInfo.CertId.ToUpper(), si.SubjectInfo.CommonName.ToUpper());
                                    if (siList.ContainsKey(siIndex))
                                    {
                                        DocsPaVO.documento.SignerInfo siTmp = siList[siIndex];
                                        si.CertificateInfo.RevocationStatusDescription = string.Empty;
                                        if (siTmp.CertificateInfo.RevocationDate != DateTime.MinValue)
                                        {
                                            si.CertificateInfo.RevocationDate = siTmp.CertificateInfo.RevocationDate;
                                            si.CertificateInfo.RevocationStatusDescription = String.Format("Il {0} ", siTmp.CertificateInfo.RevocationDate.ToShortDateString());
                                        }
                                        si.CertificateInfo.RevocationStatusDescription += new SignedDocument().DecodeStatus(siTmp.CertificateInfo.RevocationStatus);

                                        //popoliamo la revocation status SOLO se diverso da valid
                                        if ((clrExternalStatus.status != EsitoVerificaStatus.Valid))
                                            si.CertificateInfo.RevocationStatus = siTmp.CertificateInfo.RevocationStatus;
                                        else
                                            si.CertificateInfo.RevocationStatus = 0;

                                    }
                                }
                            }
                            #endregion
                            #region logica che verifica le date
                            /*
                            //Verifica le date della firma
                            DateTime endDate = si.CertificateInfo.ValidToDate;
                            if (si.CertificateInfo.RevocationDate != DateTime.MinValue)
                                endDate = si.CertificateInfo.RevocationDate;

                            //Verifica le date della firma
                            if ((dataVerifica >= si.CertificateInfo.ValidFromDate) && (dataVerifica <= endDate))
                                ValidoNelleDate = true;


                            //Max Date non è valorizzato, mettimo la data di scadenza del certificato
                            if (maxDate == DateTime.MinValue)
                                maxDate = si.CertificateInfo.ValidToDate;

                            //Min Date non è valorizzato, mettimo la data di rilascio del certificato
                            if (minDate == DateTime.MinValue)
                                minDate = si.CertificateInfo.ValidFromDate;

                            if  (si.CertificateInfo.ValidFromDate<= minDate)
                                minDate = si.CertificateInfo.ValidFromDate;

                            if (si.CertificateInfo.ValidToDate >= maxDate)
                                maxDate = si.CertificateInfo.ValidToDate;
                             */
                            #endregion

                            vsr.PKCS7Documents[x].SignersInfo[i] = si;

                            if (si.CertificateInfo.RevocationStatus != 0)
                            {
                                rc = -1;
                                break;
                            }

                            vsr.PKCS7Documents[x] = p7doc;
                        }

                    }

                    #region logica che verifica le date
                    /*
                    if (ValidoNelleDate == false)
                    {
                        rc = (int)X509ChainStatusFlags.Revoked;
                        if (dataVerifica < minDate)
                            rc = (int)X509ChainStatusFlags.CtlNotTimeValid;

                        if (dataVerifica > maxDate)
                            rc = (int)X509ChainStatusFlags.NotTimeValid;
                    }
                    else
                    {
                        rc = (int)X509ChainStatusFlags.NoError;
                    }
                     */
                    #endregion
                    vsr.StatusCode = rc;
                    vsr.StatusDescription = mainP7Doc.DecodeStatus(rc);

                }
            }
            catch (Exception e)
            {
                vsr.StatusCode = -1;
                vsr.StatusDescription = e.Message;
                logger.Debug("eccezione in verify: " + e.Message);
            }
            #endregion

            if (ErrorMessageLst.Count > 0)
            {
                vsr.ErrorMessages = ErrorMessageLst.ToArray();
                if (ErrorMessageLst.Contains("Id-AA-SigningCertificateV2 not found") ||
                    ErrorMessageLst.Contains("Missing SignedAttributes"))
                {
                    vsr.StatusCode = -5;
                    vsr.StatusDescription = "IdAASigningCertificateV2 not found";
                }
                else
                {
                    vsr.StatusCode = -1;//generico
                }
            }
            return vsr;
        }

        private DocsPaVO.documento.VerifySignatureResult PadesExternalCheck(string pathName, string file, ExternalModule ext)
        {
            //Firma PADES
            if ((Path.GetExtension(pathName).ToLower().Equals(".pdf")))
            {
                foreach (DocsPaVO.documento.PKCS7Document p in ext.Esito.VerifySignatureResult.PKCS7Documents)
                {
                    for (int ii = 0; ii < p.SignersInfo.Length; ii++)
                    {
                        DocsPaVO.documento.SignerInfo v1 = p.SignersInfo[ii];
                        if (string.IsNullOrEmpty(v1.CertificateInfo.RevocationStatusDescription))
                            v1.CertificateInfo.RevocationStatusDescription = "Valido";

                        if (v1.CertificateInfo.ThumbPrint == null)
                            v1.CertificateInfo.ThumbPrint = "Non Disponibile per la firma PADES con verifica esterna";

                        if (v1.CertificateInfo.SignatureAlgorithm == null)
                            v1.CertificateInfo.SignatureAlgorithm = "Non Disponibile per la firma PADES con verifica esterna";

                        if (v1.SubjectInfo.Nome == null)
                            v1.SubjectInfo.Nome = v1.SubjectInfo.CommonName;

                        v1.SignatureAlgorithm = "PADES";
                        p.SignersInfo[ii] = v1;
                    }
                    p.DocumentFileName = pathName;
                    p.SignHash = "Non Disponibile per la firma PADES";
                }
                ext.Esito.VerifySignatureResult.FinalDocumentName = pathName;
                ext.Esito.VerifySignatureResult.StatusCode = (int)ext.Esito.status;
                ext.Esito.VerifySignatureResult.StatusDescription = ext.Esito.message;

                //Barbatrucco per salvare il file temporaneo in locale.
                System.IO.File.WriteAllBytes(Path.Combine(this.GetPKCS7OutputDirectory(), pathName), System.IO.File.ReadAllBytes(file));
                return ext.Esito.VerifySignatureResult;
                //return vsr;
            }
            return null;
        }



        #region Lettura configurazioni dal web.config relativamente alla firma

        public string GetPKCS7InputDirectory()
        {
            string PKCS7InputDirectory = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
            PKCS7InputDirectory = System.IO.Path.Combine(PKCS7InputDirectory, @"Sign\Input\");
            return PKCS7InputDirectory;
            //return this.GetDirectoryFromSettings("PKCS7InputDirectory");
        }

        public string GetPKCS7OutputDirectory()
        {
            string PKCS7OutputDirectory = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
            PKCS7OutputDirectory = System.IO.Path.Combine(PKCS7OutputDirectory, @"Sign\Output\");
            return PKCS7OutputDirectory;
            //return this.GetDirectoryFromSettings("PKCS7OutputDirectory");
        }

        public int GetCRLTimeout()
        {
            try
            {
                return Int32.Parse(ConfigurationManager.AppSettings["RetrieveCRLTimeout"]);
            }
            catch
            {
                return (0);
            }
        }

        public bool CRLOnlineCheckEnabled()
        {
            try
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["CRLOnlineCheck"]);
            }
            catch
            {
                return false;
            }
        }

        public string CLRVerificationServiceUrl()
        {
            try
            {
                return ConfigurationManager.AppSettings["CLRVerificationModuleArgument"];
            }
            catch
            {
                return null;
            }
        }


        private string GetDirectoryFromSettings(string keyName)
        {
            string retValue = ConfigurationManager.AppSettings[keyName];

            if (retValue == string.Empty)
                throw new ApplicationException("Valore non definito per la chiave '" + keyName + "' del web.config");

            if (!retValue.EndsWith(@"\"))
                retValue += @"\";
            return retValue;
        }

        #endregion

        /// <summary>
        /// Creazione di un array di oggetti "PKCS7Document"
        /// </summary>
        /// <param name="signedDocumentArray"></param>
        /// <returns></returns>
        private DocsPaVO.documento.PKCS7Document[] CreatePKCS7DocumentArray(ArrayList signedDocumentArray)
        {
            DocsPaVO.documento.PKCS7Document[] retValue = new DocsPaVO.documento.PKCS7Document[signedDocumentArray.Count];
            int i = 0;

            foreach (SignedDocument signedDocument in signedDocumentArray)
            {
                DocsPaVO.documento.PKCS7Document document = new DocsPaVO.documento.PKCS7Document();

                document.Level = signedDocument.Level;
                document.DocumentFileName = signedDocument.DocumentFileName;
                document.SignersInfo = signedDocument.SignersInfo;
                document.SignAlgorithm = signedDocument.SignAlgorithm;
                document.SignHash = signedDocument.SignHash;
                document.SignatureType = (DocsPaVO.documento.SignType)Enum.Parse(typeof(DocsPaVO.documento.SignType), signedDocument.SignType);
                retValue[i] = document;

                document = null;
                i++;
            }

            return retValue;
        }

    }
}
