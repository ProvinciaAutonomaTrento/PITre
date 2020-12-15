using System.Xml;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using DocsPaVO.documento.Internal;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Asn1.X509;
using System.Collections;
using Org.BouncyCastle.X509.Store;
using System.Text.RegularExpressions;
using log4net;
using iTextSharp.text.pdf;


namespace SignatureVerify
{
    public class Verifica
    {
        private static ILog logger = LogManager.GetLogger(typeof(Verifica));
        public enum EsitoVerificaStatus
        {
            Valid = 0,         //OK
            NotTimeValid = 1,  //Scaduto
            Revoked = 4,       //Revocato
            Tampered = 8,
            SHA1NonSupportato = 16,
            ErroreGenerico = -1
        }

        [Serializable]
        public class EsitoVerifica
        {
            public EsitoVerificaStatus status;
            public string message;
            public string errorCode;
            public DateTime? dataRevocaCertificato;
            public string SubjectDN;
            public string SubjectCN;
            public string[] additionalData;
            public VerifySignatureResult VerifySignatureResult;
            public byte[] content;
        } 

        private  byte[] readFile(string path)
        {
            return System.IO.File.ReadAllBytes(path);
        }


       
        private PdfReader isPdf(byte[] fileContents)
        {
            try
            {
                return new PdfReader(fileContents);
                
            } catch
            {
                return null;
            }
        }
        
        public Verifica()
        {

        }

        public string VerificaFile(string fileName, string endPoint, Object[] args)
        {
            return Utils.SerializeObject<EsitoVerifica>(VerificaByteEV(readFile(fileName), endPoint, args));
        }
        public string VerificaByte(byte[] fileContents, string endPoint, Object[] args)
        {
             return Utils.SerializeObject<EsitoVerifica>(VerificaByteEV(fileContents, endPoint, args));
        }


      
        byte[] extractSignedContent(byte[] signedFile)
        {
            CmsSignedData content = new CmsSignedData(signedFile);
            CmsProcessable signedContent = content.SignedContent;
            return (byte[])signedContent.GetContent();
        }


      /// <summary>
      /// Verifiy of CRL
      /// </summary>
      /// <param name="fileContents">byte Array file contents</param>
      /// <param name="endPoint">not used </param>
      /// <param name="args">1) Datetime? data verifica / string cachePath / string (bool) nocache</param>
      /// <returns></returns>
        public EsitoVerifica VerificaByteEV(byte[] fileContents, string endPoint, Object[] args)
        {
            //string ID = String.Format("{0}-{1}", Environment.GetEnvironmentVariable("APP_POOL_ID").Replace(" ", ""), AppDomain.CurrentDomain.BaseDirectory);
            bool forceDownload = false;
            //end point lo usiamo per forzare il download 

            string p7mSignAlgorithm = null;
            //string p7mSignHash = null;
            DocsPaVO.documento.Internal.SignerInfo[] certSignersInfo;

            EsitoVerifica ev = new EsitoVerifica();

            DateTime? dataverificaDT = null;
            string cachePath = string.Empty;
            if (args == null)
            {
                logger.Debug("Args (Date) is null, settign current");
                dataverificaDT = DateTime.Now;
            }
            if (args.Length > 0)
            {
                dataverificaDT = args[0] as DateTime?;
                if (dataverificaDT == null)
                {
                    logger.Debug("Date is null, settign current");
                    dataverificaDT = DateTime.Now;
                }
                cachePath = args[1] as string;

                string fdl = args[2] as string;
                if (!String.IsNullOrEmpty(fdl))
                    Boolean.TryParse(endPoint, out forceDownload);
            }

            int posi = IndexOfInArray(fileContents, System.Text.ASCIIEncoding.ASCII.GetBytes("Mime-Version:"));
            if (posi == 0) //E' un mime m7m
            {
                using (MemoryStream ms = new MemoryStream(fileContents))
                {
                    anmar.SharpMimeTools.SharpMessage sm = new anmar.SharpMimeTools.SharpMessage(ms);
                    if (sm.Attachments.Count > 0)
                    {
                        foreach (anmar.SharpMimeTools.SharpAttachment att in sm.Attachments)
                        {
                            if (System.IO.Path.GetExtension(att.Name).ToLower().Contains("p7m"))
                            {
                                att.Stream.Position = 0;
                                BinaryReader sr = new BinaryReader(att.Stream);
                                fileContents = sr.ReadBytes((int)att.Size);
                            }
                        }
                    }
                }
            }

            // Ce provo....
            posi = -1;
            posi = IndexOfInArray(fileContents, System.Text.ASCIIEncoding.ASCII.GetBytes("%PDF"));
            if (posi == 0)    //E' un pdf
            {
                PdfReader pdfReader = isPdf(fileContents);
                try
                {
                    AcroFields af = pdfReader.AcroFields;
                    List<string> signNames = af.GetSignatureNames();

                    if (signNames.Count == 0) //Firma non è presente
                    {
                        ev.status = EsitoVerificaStatus.ErroreGenerico;
                        ev.message = "Il file PDF da verificare non contiene nessuna firma";
                        ev.errorCode = "1458";
                        return ev;
                    }

                    List<DocsPaVO.documento.Internal.SignerInfo> siList = new List<DocsPaVO.documento.Internal.SignerInfo>();
                    foreach (string name in signNames)
                    {
                        PdfPKCS7 pk = af.VerifySignature(name);
                        p7mSignAlgorithm = pk.GetHashAlgorithm();

                        Org.BouncyCastle.X509.X509Certificate[] certs = pk.Certificates;
                        foreach (X509Certificate cert in certs)
                        {
                            DocsPaVO.documento.Internal.SignerInfo si = GetCertSignersInfo(cert);

                            VerificaValiditaTemporaleCertificato(ev, dataverificaDT, cert, p7mSignAlgorithm);
                            si = ControlloCRL(forceDownload, ev, cachePath, cert, si);

                            siList.Add(si);
                        }
                        bool result = pk.Verify();
                        if (!result)
                        {
                            ev.status = EsitoVerificaStatus.ErroreGenerico;
                            ev.message = "La verifica della firma è fallita (File is Tampered)";
                            ev.errorCode = "1450";
                        }
                    }

                    /*
                    if (
                        (pdfReader.PdfVersion.ToString() != "4")||
                        (pdfReader.PdfVersion.ToString() != "7"))
                    {
                        ev.status = EsitoVerificaStatus.ErroreGenerico;
                        ev.message = "Il file da verificare non è conforme allo standard PDF 1.4 o pdf 1.7";
                        ev.errorCode = "1457";
                    }
                    */

                    List<DocsPaVO.documento.Internal.PKCS7Document> p7docsLst = new List<DocsPaVO.documento.Internal.PKCS7Document>();

                    DocsPaVO.documento.Internal.PKCS7Document p7doc = new DocsPaVO.documento.Internal.PKCS7Document
                    {
                        SignersInfo = siList.ToArray(),
                        DocumentFileName = null,
                        Level = 0
                    };
                    p7docsLst.Add(p7doc);

                    ev.VerifySignatureResult = ConvertToVerifySignatureResult(ev.status, p7docsLst.ToArray ());
                    ev.content = fileContents;
                }
                catch (Exception e)
                {
                    ev.status = EsitoVerificaStatus.ErroreGenerico;
                    ev.message = "Error verifying pdf message :" + e.Message;
                    ev.errorCode = "1402";

                    return ev;
                }
            }
            else //PKCS7
            {
                try
                {
                    int doclevel = 0;
                    List<DocsPaVO.documento.Internal.PKCS7Document> p7docsLst = new List<DocsPaVO.documento.Internal.PKCS7Document>();
                    do
                    {

                        //questa Estrazione serve solo per capire se uscire dal ciclo ricorsivo e ritornare il content
                        try
                        {
                           ev.content =  extractSignedContent(fileContents);
                        }
                        catch
                        {
                            break;
                        }

                        //Ciclo per file firmato
                        Asn1Sequence sequenza = Asn1Sequence.GetInstance(fileContents);
                        DerObjectIdentifier tsdOIDFile = sequenza[0] as DerObjectIdentifier;
                        if (tsdOIDFile != null)
                        {
                            if (tsdOIDFile.Id == CmsObjectIdentifiers.timestampedData.Id)   //TSD
                            {
                                logger.Debug("Found TSD file");
                                DerTaggedObject taggedObject = sequenza[1] as DerTaggedObject;
                                if (taggedObject != null)
                                {
                                    Asn1Sequence asn1seq = Asn1Sequence.GetInstance(taggedObject, true);
                                    TimeStampedData tsd = TimeStampedData.GetInstance(asn1seq);
                                    fileContents = tsd.Content.GetOctets();
                                }
                            }

                            if (tsdOIDFile.Id == CmsObjectIdentifiers.SignedData.Id)   //p7m
                            {
                                logger.Debug("Found P7M file");
                            }
                        }


                        CmsSignedData cms = new CmsSignedData(fileContents);
                        //controllaCrlFileP7m(cms);

                        IX509Store store = cms.GetCertificates("Collection");
                        SignerInformationStore signers = cms.GetSignerInfos();

                        SignedData da = SignedData.GetInstance(cms.ContentInfo.Content.ToAsn1Object());

                        Asn1Sequence DigAlgAsn1 = null;
                        if (da.DigestAlgorithms.Count > 0)
                            DigAlgAsn1 = da.DigestAlgorithms[0].ToAsn1Object() as Asn1Sequence;

                        if (DigAlgAsn1 != null)
                            p7mSignAlgorithm = Org.BouncyCastle.Security.DigestUtilities.GetAlgorithmName(AlgorithmIdentifier.GetInstance(DigAlgAsn1).ObjectID);

                        certSignersInfo = new DocsPaVO.documento.Internal.SignerInfo[signers.GetSigners().Count];
                        int i = 0;

                        foreach (SignerInformation signer in signers.GetSigners())
                        {

                            bool fileOK = false;
                            Org.BouncyCastle.X509.X509Certificate cert1 = GetCertificate(signer, store);
                            certSignersInfo[i] = GetCertSignersInfo(cert1);
                            VerificaValiditaTemporaleCertificato(ev, dataverificaDT, cert1, p7mSignAlgorithm);

                            fileOK= VerificaNonRepudiation(ev, fileOK, cert1);
                            if (!fileOK)
                                certSignersInfo[i].CertificateInfo.messages = ev.errorCode+" " + ev.message;

                            try
                            {
                                fileOK = VerificaCertificato(ev, signer, fileOK, cert1);

                            }
                            catch (Exception e)
                            {
                                ev.status = EsitoVerificaStatus.ErroreGenerico;
                                ev.message = "Error verifying 2, message :" + e.Message;
                                ev.errorCode = "1450";
                            }

                            
                            if (fileOK)
                                certSignersInfo[i] = ControlloCRL(forceDownload, ev, cachePath, cert1, certSignersInfo[i]);

                            //p7mSignHash = BitConverter.ToString(Org.BouncyCastle.Security.DigestUtilities.CalculateDigest(Org.BouncyCastle.Security.DigestUtilities.GetAlgorithmName(AlgorithmIdentifier.GetInstance(DigAlgAsn1).ObjectID), (byte[])cms.SignedContent.GetContent())).Replace("-", "");
                        }
                        /*
                        if (cms.SignedContent != null)
                        {
                            //CmsProcessable signedContent = cms.SignedContent;
                            //ev.content = (byte[])signedContent.GetContent();

                            ev.content = extractMatrioskaFile(fileContents);

                           

                        }
                         */


                        DocsPaVO.documento.Internal.PKCS7Document p7doc = new DocsPaVO.documento.Internal.PKCS7Document
                        {
                            SignersInfo = certSignersInfo,
                            DocumentFileName = null,
                            Level = doclevel++
                        };
                        p7docsLst.Add(p7doc);
                        try
                        {
                            fileContents = extractSignedContent(fileContents);
                        }
                        catch
                        {
                            break;
                        }
                        

                    } while (true);

                    ev.VerifySignatureResult = ConvertToVerifySignatureResult(ev.status, p7docsLst.ToArray ()); ;
                    
                }
                catch (Exception e)
                {
                    ev.status = EsitoVerificaStatus.ErroreGenerico;
                    ev.message = "Error verifying 1, message :" + e.Message;
                    ev.errorCode = "1402";

                    return ev;
                }
            }
            return ev;
        }

        private static bool VerificaNonRepudiation(EsitoVerifica ev, bool fileOK, Org.BouncyCastle.X509.X509Certificate cert1)
        {


            /*    
digitalSignature        (0),
nonRepudiation          (1),
keyEncipherment         (2),
dataEncipherment        (3),
keyAgreement            (4),
keyCertSign             (5),
cRLSign                 (6),
encipherOnly            (7),
decipherOnly            (8)
*/
            try
            {
                bool[] keyUsageBA = cert1.GetKeyUsage();
                if (!keyUsageBA[1]) // Manca la no repudiation non va bene per i file firmati!!!
                {
                    fileOK = false;
                    ev.status = EsitoVerificaStatus.ErroreGenerico;
                    ev.message = "La chiave certificata non e abilitata alla firma";
                    ev.errorCode = "1409";
                }
            }
            catch
            {
            }
            return fileOK;
        }

        private static bool VerificaCertificato(EsitoVerifica ev, SignerInformation signer, bool fileOK, Org.BouncyCastle.X509.X509Certificate cert1)
        {
            fileOK = signer.Verify(cert1);

            if (signer.SignedAttributes != null)
            {
                if (signer.SignedAttributes[Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.IdAASigningCertificateV2] == null)
                {
                    fileOK = false;
                    ev.status = EsitoVerificaStatus.ErroreGenerico;
                    ev.message = "Il formato dell'attributo signingCertificateV2 non è presente";
                    ev.errorCode = "145C";
                }
            }
            else
            {
                fileOK = false;
                ev.status = EsitoVerificaStatus.ErroreGenerico;
                ev.message = "Mancano le signed Attributes obbligatorie( IdAAsigningCertificateV2, MessageDigest, contentType )";
                ev.errorCode = "1450";
            }
            return fileOK;
        }


        public static int IndexOfInArray(byte[] array, byte[] pattern)
        {
            bool found = false;

            if (pattern.Length > array.Length)
                return -1;

            int i, j;

            for (i = 0, j = 0; i < array.Length; )
            {
                if (array[i++] != pattern[j++])
                {
                    j = 0;
                    continue;
                }

                if (j == pattern.Length)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                return -1;
            else
                return i - pattern.Length;
        }



        private DocsPaVO.documento.Internal.SignerInfo ControlloCRL(bool forceDownload, EsitoVerifica ev, string cachePath, X509Certificate cert, DocsPaVO.documento.Internal.SignerInfo si)
        {
            //il controllo CRL lo facciamo alla fine.. dato che è dispendioso
            if (ev.status == EsitoVerificaStatus.Valid)
            {
                EsitoVerifica evCrl = controllaCrlCert(cert, cachePath, forceDownload);
                si.CertificateInfo.RevocationStatus = (int)evCrl.status;
                si.CertificateInfo.RevocationStatusDescription = evCrl.status.ToString();
                if (evCrl.dataRevocaCertificato != null)
                {
                    si.CertificateInfo.RevocationDate = evCrl.dataRevocaCertificato.Value;
                    ev.dataRevocaCertificato = evCrl.dataRevocaCertificato;
                }

                if (evCrl.status != EsitoVerificaStatus.Valid)
                {
                    ev.status = evCrl.status;
                    ev.message = evCrl.message;
                    ev.errorCode = evCrl.errorCode;
                }
            }
            return si;
        }

        private static void VerificaValiditaTemporaleCertificato(EsitoVerifica ev, DateTime? dataverificaDT, Org.BouncyCastle.X509.X509Certificate certificate, string SignAlgorithm)
        {
            //certificato scaduto
            if (certificate.NotAfter.ToLocalTime() < dataverificaDT.Value)
            {
                logger.DebugFormat("Expired Certificate {0} > Expiry Date [{1}]", dataverificaDT.Value, certificate.NotAfter.ToLocalTime());
                ev.status = EsitoVerificaStatus.NotTimeValid;
                ev.errorCode = "1407";
            }

            //certificato ancora non valido
            if (certificate.NotBefore.ToLocalTime() > dataverificaDT.Value)
            {
                logger.DebugFormat("Not yet valid Certificate {0} > Start Date[{1}]", dataverificaDT.Value, certificate.NotBefore.ToLocalTime());
                ev.status = EsitoVerificaStatus.NotTimeValid;
                ev.errorCode = "1426";
            }

            if (SignAlgorithm.ToLower().Contains("sha1") ||
                       SignAlgorithm.ToLower().Contains("sha-1")
                       )
            {
                //dal 30 11 2011 lo sha1 non è piu conforme alla normativa
                if (dataverificaDT.Value > DateTime.Parse("2011-07-01T00:00:00+00:00"))
                {
                    logger.Debug("Sha-1 used AFTER 2011-06-30");
                    ev.status = EsitoVerificaStatus.SHA1NonSupportato;
                    ev.errorCode = "1468";
                }
            }
        }

        private DocsPaVO.documento.Internal.SignerInfo GetCertSignersInfo(Org.BouncyCastle.X509.X509Certificate cert1)
        {
            DocsPaVO.documento.Internal.SignerInfo retval= new DocsPaVO.documento.Internal.SignerInfo ();
            retval.SubjectInfo = new SubjectInfo();
            string Subject = buildSubject(cert1.SubjectDN);

            ParseCNIPASubjectInfo(ref retval.SubjectInfo, Subject);
            retval.CertificateInfo.IssuerName = cert1.IssuerDN.ToString();
            retval.CertificateInfo.SerialNumber = BitConverter.ToString(cert1.SerialNumber.ToByteArray()).Replace("-", "");
            retval.CertificateInfo.SignatureAlgorithm = cert1.SigAlgName;
            retval.CertificateInfo.SubjectName = Subject;
            retval.CertificateInfo.ValidFromDate = cert1.NotBefore.ToLocalTime();
            retval.CertificateInfo.ValidToDate = cert1.NotAfter.ToLocalTime();
            retval.CertificateInfo.ThumbPrint = BitConverter.ToString(System.Security.Cryptography.SHA1.Create().ComputeHash(cert1.GetEncoded())).Replace("-", "");
            return retval;
        }


        private static VerifySignatureResult ConvertToVerifySignatureResult(EsitoVerificaStatus status, DocsPaVO.documento.Internal.PKCS7Document[] p7docsLst)
        {
            VerifySignatureResult vsr = new VerifySignatureResult();
            vsr.PKCS7Documents = p7docsLst;
            vsr.StatusCode = (int)status;
            vsr.StatusDescription = status.ToString();
            vsr.CRLOnlineCheck = true;
            return vsr;
        }

        #region cnipa

        private string buildSubject( X509Name SubjectDN)
        {
            return String.Format("dnQualifier={0}, SN={1}, G={2}, SERIALNUMBER={3}, CN={4}, O={5}, C={6}",
                SubjectDN.GetValues(new DerObjectIdentifier("2.5.4.46")).Cast<string>().FirstOrDefault(),
                SubjectDN.GetValues(new DerObjectIdentifier("2.5.4.4")).Cast<string>().FirstOrDefault(),
                SubjectDN.GetValues(new DerObjectIdentifier("2.5.4.42")).Cast<string>().FirstOrDefault(),
                SubjectDN.GetValues(new DerObjectIdentifier("2.5.4.5")).Cast<string>().FirstOrDefault(),
                SubjectDN.GetValues(new DerObjectIdentifier("2.5.4.3")).Cast<string>().FirstOrDefault(),
                SubjectDN.GetValues(new DerObjectIdentifier("2.5.4.10")).Cast<string>().FirstOrDefault(),
                SubjectDN.GetValues(new DerObjectIdentifier("2.5.4.6")).Cast<string>().FirstOrDefault()
                );
            
        }
        
        /// <summary>
        /// Parse Certificate Description
        /// </summary>
        /// <param name="sd">a structure filled with results</param>
        /// <param name="subject">the input string</param>
        private void ParseCNIPASubjectInfo(ref DocsPaVO.documento.Internal.SubjectInfo subjectInfo, string subject)
        {
            Regex r = new Regex(".*?CN=(?<cognome>.*?)/(?<nome>.*?)/(?<cf>.*?)/(?<cid>.*?),.*", RegexOptions.IgnoreCase);
            Match match = r.Match(subject);

            if (match.Success)
            {
                // Formato supportato dalla normativa 2000
                string commonName = match.Groups["cognome"].Value + " " + match.Groups["nome"].Value;
                if (commonName.Trim() != string.Empty)
                    subjectInfo.CommonName = commonName;

                subjectInfo.CodiceFiscale = match.Groups["cf"].Value;
                subjectInfo.CertId = match.Groups["cid"].Value;
            }

            r = new Regex(@"Description=\""C=(?<cognome>.*?)/N=(?<nome>.*?)/D=(?<g>\d+)-(?<m>\d+)-(?<a>\d+)(/R=(?<r>.*?))?\""(.*C=(?<country>.*?)$)?", RegexOptions.IgnoreCase);
            match = r.Match(subject);

            if (match.Success)
            {
                // Formato supportato dalla normativa 2000
                subjectInfo.Cognome = match.Groups["cognome"].Value;
                subjectInfo.Nome = match.Groups["nome"].Value;
                subjectInfo.DataDiNascita =
                    new DateTime(Int32.Parse(match.Groups["a"].Value),
                                 Int32.Parse(match.Groups["m"].Value),
                                 Int32.Parse(match.Groups["g"].Value)).ToShortDateString();

                subjectInfo.Ruolo = match.Groups["r"].Value;
                subjectInfo.Country = match.Groups["country"].Value;
            }
            else
            {
                // Formato supportato dalla normativa del 17/02/2005
                Hashtable subjectItems = this.ParseCNIPASubjectItems(subject);

                subjectInfo.CommonName = this.GetSubjectItem(subjectItems, "CN");
                subjectInfo.Cognome = this.GetSubjectItem(subjectItems, "SN");
                subjectInfo.Nome = this.GetSubjectItem(subjectItems, "G");
                subjectInfo.CertId = this.GetSubjectItem(subjectItems, "DNQUALIFIER");
                subjectInfo.SerialNumber = this.GetSubjectItem(subjectItems, "SERIALNUMBER");
                // Se il country code è "IT", il "SerialNumber" contiene il codice fiscale del titolare del certificato
                if (subjectInfo.SerialNumber.StartsWith("IT:"))
                    subjectInfo.CodiceFiscale = subjectInfo.SerialNumber.Substring(subjectInfo.SerialNumber.IndexOf(":") + 1);
                subjectInfo.Country = this.GetSubjectItem(subjectItems, "C");
                subjectInfo.Organizzazione = this.GetSubjectItem(subjectItems, "O");
                subjectItems.Clear();
                subjectItems = null;
            }
        }

        /// <summary>
        /// Estrazione dei singoli elementi che compongono
        /// la descrizione del firmatario del certificato
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        private Hashtable ParseCNIPASubjectItems(string subject)
        {
            Hashtable retValue = new Hashtable();

            string[] items = subject.Split(',');
            Regex regex = new Regex("[A-Za-z]*=[^\"][^,]*");
            MatchCollection matchColl = regex.Matches(subject);
            for (int i = 0; i < matchColl.Count; i++)
            {
                string item = matchColl[i].Value;
                int indexOfValue = item.IndexOf("=");
                string itemKey = item.Substring(0, indexOfValue).ToUpper().Trim();
                string itemValue = item.Substring(indexOfValue + 1).ToUpper().Trim();
                retValue[itemKey] = itemValue;
            }
            Regex regex2 = new Regex("[A-Za-z]*=\"[\\d\\D]*\"");
            MatchCollection matchColl2 = regex2.Matches(subject);
            for (int i = 0; i < matchColl2.Count; i++)
            {
                string item = matchColl2[i].Value;
                int indexOfValue = item.IndexOf("=");
                string itemKey = item.Substring(0, indexOfValue).ToUpper().Trim();
                string itemValue = item.Substring(indexOfValue + 1).ToUpper().Trim();
                retValue[itemKey] = itemValue;
            }

            return retValue;
        }
        #endregion


        private string GetSubjectItem(Hashtable subjectItems, string itemKey)
        {
            if (subjectItems.ContainsKey(itemKey))
                return subjectItems[itemKey].ToString();
            else
                return string.Empty;
        }



        private Org.BouncyCastle.X509.X509Certificate GetCertificate(SignerInformation signer, IX509Store store)
        {
            X509CertStoreSelector sel = new X509CertStoreSelector();
            sel.SerialNumber = signer.SignerID.SerialNumber;
            ICollection coll = store.GetMatches(sel);
            IEnumerator en = coll.GetEnumerator();
            en.MoveNext();
            return en.Current as Org.BouncyCastle.X509.X509Certificate;
        }

        List<EsitoVerifica> controllaCrlFileP7m(CmsSignedData sd)
        {

            List<EsitoVerifica> verificheLst = new List<EsitoVerifica>();
            SignedData da = SignedData.GetInstance(sd.ContentInfo.Content.ToAsn1Object());
            foreach (DerSequence cer in da.Certificates)
            {
                X509CertificateParser cp = new X509CertificateParser();
                X509Certificate cert = cp.ReadCertificate(cer.GetEncoded());
                verificheLst.Add(controllaCrlCert(cert,null,false));
            }
            return verificheLst;
        }

        EsitoVerifica controllaCrlCert(X509Certificate cert,string cachePath,bool force=false)
        {
            //usiamo l'ev solo per i dati di revoca
            EsitoVerifica ev = new EsitoVerifica();
            string CN = cert.SubjectDN.GetValues(X509Name.CN).Cast<string>().FirstOrDefault ();
            string SN = cert.SubjectDN.GetValues(X509Name.SerialNumber).Cast<string>().FirstOrDefault ();
            X509Extensions ex = X509Extensions.GetInstance(cert.CertificateStructure.TbsCertificate.Extensions);
            X509Extension e = ex.GetExtension(X509Extensions.CrlDistributionPoints);
            if (e == null)
            {
                string msg = "CRL distribution points NOT PRESENT in certificate structure";
                logger.Debug(msg);
                ev.status = EsitoVerificaStatus.ErroreGenerico;
                ev.errorCode = "1411";//nonposso scaricare la CRL
                ev.message = msg;
                return ev;
            }
            var crldp = CrlDistPoint.GetInstance(e.GetParsedValue());
            List<String> certDpUrlLst = GetCrlDistribtionPoints(crldp);
            ev.status = EsitoVerificaStatus.Valid;
            ev.SubjectCN = CN;
            ev.SubjectDN = SN;
            int downloadsTrials = 0;
            
            List<String> errorLst = new List<string>();
            foreach (string url in certDpUrlLst)
            {
                try
                {
                    Uri tryUri = new Uri(url);
                }
                catch
                {
                    logger.ErrorFormat("Unable to download/process CRL  URL : {0}",url); 
                    continue; 
                }
                
                try
                {
                    X509Crl rootCrl = retreiveCrlUrl(url,cachePath,force);
                    downloadsTrials++;
                    if (rootCrl.IsRevoked(cert))
                    {
                        X509CrlEntry entry = rootCrl.GetRevokedCertificate(cert.CertificateStructure.SerialNumber.Value);
                        ev.dataRevocaCertificato = entry.RevocationDate;
                        logger.DebugFormat("Certificate {0} : {1} with serial {2}  is Revoked on {3}",CN,SN,BitConverter.ToString (entry.SerialNumber.ToByteArray()), ev.dataRevocaCertificato);
                        ev.content  = entry.SerialNumber.ToByteArray();
                        ev.errorCode = "1408";
                        ev.status = EsitoVerificaStatus.Revoked;
                        
                        break;
                    }
                }
                catch (Exception exc)
                {
                    logger.ErrorFormat("Unable to download/process CRL message {0} stack {1} on Download Trial {2}", exc.Message, exc.StackTrace, downloadsTrials);
                    errorLst.Add(exc.Message);
                }

            }
            
            string ErrorMessage = string.Empty;
            if ((errorLst.Count >0)&& downloadsTrials ==0)
            {
                foreach (string s in errorLst)
                ErrorMessage += s + " | ";
            }


            if  (!string.IsNullOrEmpty( ErrorMessage))
            {
                ev.status = EsitoVerificaStatus.ErroreGenerico;
                ev.errorCode = "1411";//nonposso scaricare la CRL
                ev.message = "Unable to download/process CRL message:" + ErrorMessage;
            }


            return ev;
        }

        private static List<String> GetCrlDistribtionPoints(CrlDistPoint crldp)
        {
            List<String> certDpUrlLst = new List<string>();
            DistributionPoint[] dpLst = crldp.GetDistributionPoints();
            foreach (DistributionPoint p in dpLst)
            {
                GeneralName[] names = GeneralNames.GetInstance(p.DistributionPointName.Name).GetNames();
                foreach (GeneralName n in names)
                    certDpUrlLst.Add(GeneralName.GetInstance(n).Name.ToString());
            }
            return certDpUrlLst;
        }

        private X509Crl retreiveCrlUrl(string url,string cachePath, bool force = false)
        {
            X509Crl rootCrl = null;
            RetreiveCRL.GetCRL client =new RetreiveCRL.GetCRL();
            client.CachePath = cachePath;
            
            if (force)
                client.NoCache = true;

            client.GetCRLFromDistributionList(url);
            if (client.CertificationRevocationListBinary != null)
            {
                byte[] contCRL = client.CertificationRevocationListBinary;
                X509CrlParser crlParser = new X509CrlParser();
                //Carico la CRL nel reader
                rootCrl = crlParser.ReadCrl(contCRL);

                if (rootCrl.NextUpdate.Value.ToLocalTime() < DateTime.Now) //la crl è scaduta la riscarico.
                {
                    client.NoCache = true;
                    client.GetCRLFromDistributionList(url);
                    contCRL = client.CertificationRevocationListBinary;
                    rootCrl = crlParser.ReadCrl(contCRL);
                }
            }
            return rootCrl;
        }

        /// <summary>
        /// Verifica singolo Certificato 
        /// </summary>
        /// <param name="CertificateDer">Bytearray del certificato x509</param>
        /// <param name="CertificateCAPEM">non usato</param>
        /// <param name="args">parametri di oggetti opzionali</param>
        /// <returns>Ritorna lo status della verifica.</returns>
        public EsitoVerifica VerificaCertificato(byte[] CertificateDer, byte[] CertificateCAPEM, Object[] args)
        {
            EsitoVerifica retval = null;
            string cachePath;
            bool forceDownload = false;
            if (args.Length > 0)
            {
                cachePath = args[1] as string;

                string fdl = args[2] as string;
                if (!String.IsNullOrEmpty(fdl))
                    Boolean.TryParse(fdl, out forceDownload);

                //funzioni bouncycastle per estrapolare il certificato dal binario
                X509CertificateParser cp = new X509CertificateParser();
                X509Certificate cert = cp.ReadCertificate(CertificateDer);

                List<DocsPaVO.documento.Internal.SignerInfo> retSI = new List<DocsPaVO.documento.Internal.SignerInfo>();
                List<PKCS7Document> p7doc = new List<PKCS7Document>();
                retSI.Add(GetCertSignersInfo(cert));
                p7doc.Add(new PKCS7Document { SignersInfo = retSI.ToArray() });

                //Questo scarica la CRL e fa la verifica.
                retval = controllaCrlCert(cert, cachePath, forceDownload);
                int statusInt = (int)retval.status;
                retval.VerifySignatureResult = new VerifySignatureResult { StatusCode = statusInt, PKCS7Documents = p7doc.ToArray()};

            }
            return retval;
        }

        public CertificateInfo GetCertificateInfoFromEv(EsitoVerifica ev)
        {
            //grande assunto
            try
            {
                return ev.VerifySignatureResult.PKCS7Documents[0].SignersInfo[0].CertificateInfo;
            }
            catch
            {
                return new CertificateInfo();
            }
        }
    }

    public class Utils
    {
        static String UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        public static String SerializeObject<t>(Object pObject)
        {
            try
            {
                String XmlizedString = null;
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(typeof(t));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                xs.Serialize(xmlTextWriter, pObject, ns);
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
                return XmlizedString;
            }
            catch (Exception e) { System.Console.WriteLine(e); return null; }
        }
    }


}
