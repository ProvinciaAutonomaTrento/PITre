using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using VtDocsWS.WebServices;


namespace VtDocsWS.Manager
{
    public class SignatureManager
    {
        public static Services.Signature.VerifySignature.VerifySignatureResponse VerifySignature(Services.Signature.VerifySignature.VerifySignatureRequest request)
        {
            Services.Signature.VerifySignature.VerifySignatureResponse response = new Services.Signature.VerifySignature.VerifySignatureResponse();

            // TO DO: corpo del metodo di verifica firma

            /*
             * Corpo del metodo vero e proprio.
             * Inizialmente dovrebbe andarci il corpo del metodo VerificaValiditaFirma del .asmx --> Quello immediatamente sotto
             * Poi il codice di Alessandro Faillace.
             */
 
            /*
             * 
              SetUserId(infoUtente);
              DocsPaVO.documento.FileDocumento fileDocumento = null;

              try
              {

              #if TRACE_WS
			  DocsPaUtils.LogsManagement.PerformaceTracer pt = new DocsPaUtils.LogsManagement.PerformaceTracer("TRACE_WS");
              #endif
              
              DateTime dataRif = BusinessLogic.Documenti.FileManager.dataRiferimentoValitaDocumento(fileRequest, infoUtente);
              bool con;
              fileDocumento = BusinessLogic.Documenti.FileManager.getFile(fileRequest, infoUtente, false, false, out con);
              BusinessLogic.Documenti.FileManager.VerifyFileSignature(fileDocumento, dataRif);
    
              #if TRACE_WS
			  pt.WriteLogTracer("VerificaValiditaFirma");
              #endif
              }
              catch (Exception e)
              {
                  logger.Debug("Errore in DocsPaWS.asmx  - metodo: VerificaValiditaFirma", e);
              }
            
              //return fileDocumento;
            *  
            */
              
            //GetFileDoc - DA VERIFICARE E ANALIZZARE TRY - CATCH E VALORI DI RITORNO
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "VerifySignature");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.IdDocument))
                {
                    throw new PisException("REQUIRED_ID");
                }

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();

                try
                {
                    if (!string.IsNullOrEmpty(request.IdDocument))
                    {
                        documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDocument, request.IdDocument);
                        if (documento != null && documento.documenti != null && documento.documenti.Count > 0)
                        {
                            int numVersione = 0;
                            if (!string.IsNullOrEmpty(request.VersionId))
                            {
                                bool result = Int32.TryParse(request.VersionId, out numVersione);
                                if (!result)
                                {
                                    throw new PisException("REQUIRED_INTEGER");
                                }
                                else
                                {
                                    if (documento.documenti.Count < numVersione || numVersione <= 0)
                                    {
                                        throw new PisException("FILE_VERSION_NOT_FOUND");
                                    }
                                    else
                                    {
                                        numVersione = documento.documenti.Count - numVersione;
                                    }
                                }
                            }
                            DocsPaVO.documento.FileRequest versione = (DocsPaVO.documento.FileRequest)documento.documenti[numVersione];
                            response.FileDoc = Utils.GetFileDoc(versione, true, infoUtente, false, false, string.Empty, null);
                        }
                        else
                        {
                            throw new PisException("DOCUMENT_NOT_FOUND");
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }

                response.Success = true;
            }
            catch (PisException pisEx)
            {
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        
        

        /*
         * Codice di Alessandro Faillace da inserire in futuro in VerifySignature
         */

        /*
         * 
           static void  ControllaCRL (string p7mFile)
           {
                 byte[] cont = System.IO.File.ReadAllBytes(p7mFile);
                 CmsSignedData sd = new CmsSignedData(cont);
                 SignedData da = SignedData.GetInstance(sd.ContentInfo.Content.ToAsn1Object());
                 foreach (DerSequence cer in da.Certificates)
                 {

                     X509CertificateParser cp = new X509CertificateParser();
                     X509Certificate cert = cp.ReadCertificate(cer.GetEncoded());
                     ArrayList  CN = cert.SubjectDN.GetValues(X509Name.CN);
                     X509Extensions ex= X509Extensions.GetInstance (cert.CertificateStructure .TbsCertificate.Extensions);

                     X509Extension e = ex.GetExtension(X509Extensions.CrlDistributionPoints);
                     var crldp = CrlDistPoint.GetInstance(e.GetParsedValue());
                     DistributionPoint[] dpLst=   crldp.GetDistributionPoints();

                     //System.Net.WebClient Client = new WebClient();
                 
                     foreach (DistributionPoint p in dpLst)
                     {
                         GeneralName[] names = GeneralNames.GetInstance(p.DistributionPointName.Name).GetNames();
                         foreach (GeneralName n in names)
                         {
                             GeneralName name = GeneralName.GetInstance (n);
                             string url = name.Name.ToString();
                             RetreiveCRL.GetCRL client = new RetreiveCRL.GetCRL(url);

                             if (client.CertificationRevocationListBinary != null)
                             {
                                 byte[] contCRL = client.CertificationRevocationListBinary;
                                 X509CrlParser crlParser = new X509CrlParser();
                                 X509Crl rootCrl = crlParser.ReadCrl(contCRL);
                                 bool revo = rootCrl.IsRevoked(cert);
                             }
                             else
                             {
                                 //non sono riuscito a scaricare la CLR.. decidere il return type
                             }
                         }
                     }
                 
                     string CNNAME=string.Empty;
                     foreach (string c in CN)
                     {
                         CNNAME += c + " ";
                     }

                     Console.Write(p7mFile + " "); 
                     Console.Write(cert.SigAlgName+ " "  ); 
                     Console.WriteLine(CNNAME);
                
                 }

            }

         
            static void CreateTSD(string p7mFile, string[] tsrFiles,string tsdFile)
            {
                byte[] cont = System.IO.File.ReadAllBytes(p7mFile);
                Asn1OctetString p7mOctecString = (Asn1OctetString)new DerOctetString(cont).ToAsn1Object();
        
                //TimeStamp Stuff:
                List<TimeStampAndCrl> tsCRLLst = new List<TimeStampAndCrl>();
                foreach (string tsrFile in tsrFiles)
                {
                    byte[] contTSR = System.IO.File.ReadAllBytes(tsrFile);
                    TimeStampResponse tsr = new TimeStampResponse(contTSR);
                    TimeStampAndCrl tsCRL = new TimeStampAndCrl(tsr.TimeStampToken.ToCmsSignedData().ContentInfo);
                    tsCRLLst.Add(tsCRL);
                }

                Evidence ev = new Evidence(new TimeStampTokenEvidence(tsCRLLst.ToArray()));
                TimeStampedData newTSD = new TimeStampedData(null,null, p7mOctecString, ev);
                ContentInfo info = new ContentInfo(CmsObjectIdentifiers.timestampedData, newTSD.ToAsn1Object());
                System.IO.File.WriteAllBytes(tsdFile, info.GetDerEncoded());

            }

            static void ExractFromTSD(string file)
            {
                byte[] cont = System.IO.File.ReadAllBytes(file);
         
                string path = System.IO.Path.GetDirectoryName (file);
                string fileName = System.IO.Path.Combine ( path,System.IO.Path.GetFileNameWithoutExtension(file));

                Asn1Sequence sequenza = Asn1Sequence.GetInstance(cont);
                DerObjectIdentifier tsdOIDFile = sequenza[0] as DerObjectIdentifier;
                if (tsdOIDFile != null)
                {
                    if (tsdOIDFile.Id == CmsObjectIdentifiers.timestampedData.Id)
                    {
                        DerTaggedObject taggedObject = sequenza[1] as DerTaggedObject;
                        if (taggedObject != null)
                        {
                            Asn1Sequence asn1seq = Asn1Sequence.GetInstance(taggedObject, true);
                            TimeStampedData tsd = TimeStampedData.GetInstance(asn1seq);

                            string p7mFile = String.Format("{0}", fileName);
                            System.IO.File.WriteAllBytes(p7mFile, tsd.Content.GetOctets());
                            Org.BouncyCastle.Cms.CmsSignedData d = new CmsSignedData(tsd.Content.GetOctets());
                            byte[] sbustato = (byte[])d.SignedContent.GetContent();
                            string plainFileName = System.IO.Path.Combine(path, System.IO.Path.GetFileNameWithoutExtension(fileName));
                            System.IO.File.WriteAllBytes(plainFileName, sbustato);

                            TimeStampAndCrl[] crlTS = tsd.TemporalEvidence.TstEvidence.ToTimeStampAndCrlArray();

                            foreach (TimeStampAndCrl tokCRL in crlTS)
                            {
                                TimeStampToken tsToken = new TimeStampToken(tokCRL.TimeStampToken);
                                ContentInfo o = tokCRL.TimeStampToken;

                                Org.BouncyCastle.Asn1.Cmp.PkiStatusInfo si = new Org.BouncyCastle.Asn1.Cmp.PkiStatusInfo(0);
                                Org.BouncyCastle.Asn1.Tsp.TimeStampResp re = new Org.BouncyCastle.Asn1.Tsp.TimeStampResp(si, o);

                                //SCRIVO IL TSR
                                string tsrFile = String.Format("{0}.{1}.tsr", plainFileName, tsToken.TimeStampInfo.SerialNumber.ToString());
                                System.IO.File.WriteAllBytes(tsrFile, re.GetEncoded());

                            }
                        }
                    }
                }

            }
         * 
         */

    }
}