using System;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.X509.Store;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1.Cms;
using DocsPaVO.documento;
using System.Collections.Generic;

namespace BusinessLogic.Documenti.DigitalSignature
{
    public class SignedDocument
    {
        #region Properties
        private bool gestioneTSFirma = true; //mettere a true per gestire il timestamp nella firma pkcs
        // hashtable contenente le descrizioni degli algoritmi RSA utilizzati
        private Hashtable _algHT = new Hashtable();

        protected byte[] _buf = null;
        protected string _pathName = null;
        protected string _documentFileName = null;
        protected string _SignAlgorithm = null;
        protected string _SignType=null;
        protected string _SignHash = null;
        protected DocsPaVO.documento.SignerInfo[] _signersInfo;
        protected byte[] _content = null;
        protected int _level = 0;

        protected bool _enableASN1UnstructuredNameUse = true;
        protected bool _crlOnlineCheck = false;
        protected int _crlRetrieveTimeout = 5000;

        public int Level
        {
            get { return (_level); }
            set { _level = value; }
        }

        public string SignType
        { get { return (_SignType); } set { } }

        public string SignAlgorithm
        { get { return (_SignAlgorithm); } set { } }

        public string SignHash
        { get { return (_SignHash); } set { } }

        public string DocumentFileName
        { get { return (_documentFileName); } set { } }

        [XmlIgnore]
        public byte[] Buffer
        { get { return (_buf); } }

        [XmlIgnore]
        public string PathName
        { get { return (_pathName); } }

        //[XmlIgnore]
        //public bool IsCounterSigned
        //{
        //    get
        //    {
        //        bool rc = false;
        //        if (_buf != null)
        //            rc = CryptoHelper.IsPKCS7CounterSigned(_buf, _buf.Length);
        //        return (rc);
        //    }
        //}

        public DocsPaVO.documento.SignerInfo[] SignersInfo
        { get { return _signersInfo; } set { } }

        [XmlIgnore]
        public byte[] Content
        { get { return _content; } }

        [XmlIgnore]
        public bool EnableASN1UnstructuredNameUse
        {
            get { return (_enableASN1UnstructuredNameUse); }
            set { _enableASN1UnstructuredNameUse = value; }
        }

        [XmlIgnore]
        public bool CRLOnlineCheck
        {
            get { return (_crlOnlineCheck); }
            set { _crlOnlineCheck = value; }
        }

        [XmlIgnore]
        public int CRLRetrieveTimeout
        {
            get { return (_crlRetrieveTimeout); }
            set { _crlRetrieveTimeout = value; }
        }

        #endregion


        /// <summary>
        /// Default constructor
        /// </summary>
        public SignedDocument()
        {
            this.FillTableAlgorithm();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="buf">buf to allocate</param>
        /// <param name="pathName">pathName to identify file</param>
        public SignedDocument(byte[] buf, string pathName)
            : this()
        {
            if (buf != null)
            {
                _buf = buf;
                _pathName = pathName;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pathName">PKCS7 fileName to open</param>
        public SignedDocument(string pathName)
            : this()
        {
            LoadFromFile(pathName);
        }

        /// <summary>
        /// Loads a PKCS7 file in memory
        /// </summary>
        /// <param name="pathName">file to load</param>
        /// <returns>true if load succeded, false otherwise</returns>
        public bool LoadFromFile(string pathName)
        {
            bool rc = false;
            _buf = null;

            FileStream fsi = null;
            try
            {
                fsi = new FileStream(pathName, FileMode.Open, FileAccess.Read, FileShare.Read);
                long fileLength = fsi.Length;
                _buf = new byte[fileLength];
                fsi.Read(_buf, 0, (int)fileLength);
                _pathName = pathName;
                rc = true;
            }
            catch (FileNotFoundException fne)
            {
                throw new FileNotFoundException(fne.Message);
            }
            catch (Exception e)
            {
                _buf = null;
                throw new ApplicationException(e.Message);
            }
            finally
            {
                if (fsi != null) fsi.Close();
            }
            return (rc);
        }

        /// <summary>
        /// Retrieves the document name embedded in PKCS7. If ASN1 upstructuredName option is enabled
        /// the document name i retrieved from this extension (if present), otherwise the document name
        /// is retrieved from the origial document name stripping the extension.
        /// </summary>
        /// <returns></returns>
        private string GetDocumentFileName()
        {
            string fileName = String.Empty;
            //if (_enableASN1UnstructuredNameUse)
            //{
            //    string unstructuredName = this.GetASN1UnstructuredName();
            //    if (unstructuredName != String.Empty)
            //    {
            //        // extract filename from upstructuredname string ...
            //        Regex r = new Regex(@"(?<Keyword>.*?)\s*=\s*(?<Value>.*?);", RegexOptions.IgnoreCase);
            //        Hashtable ht = new Hashtable();
            //        Match m = r.Match(unstructuredName);
            //        if (m.Success)
            //        {
            //            while (m.Success)
            //            {
            //                ht.Add(m.Groups[1].Value.ToLower(), m.Groups[2].Value);
            //                m = m.NextMatch();
            //            }
            //            fileName = ht["filename"].ToString();
            //        }
            //    }
            //}
            //if (fileName == String.Empty)
            //{
            // Extract fileName from pathName removing the most external .P7M extension ...
            string tmp = _pathName;
            if (Path.GetExtension(tmp).ToLower() == ".p7m")
            {
                tmp = Path.GetFileNameWithoutExtension(tmp);
            }
            fileName = tmp;
            //}

            return fileName;
        }

        ///// <summary>
        ///// Get ASN.1 ustructuredName attribute (if present)
        ///// </summary>
        ///// <returns></returns>
        //private string GetASN1UnstructuredName()
        //{
        //    // Get embedded file name ...
        //    StringBuilder unstructuredName = new StringBuilder(1024); // Allocate string space
        //    CryptoHelper.GetASN1UnstructuredName
        //        (unstructuredName, unstructuredName.Capacity, _buf, _buf.Length);

        //    return (unstructuredName.Length > 0 ? unstructuredName.ToString() : String.Empty);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        protected virtual byte[] GetSignedContent(byte[] content)
        {
            byte[] signedContent = null;

            using (StreamReader reader = new StreamReader(new MemoryStream(content)))
            {
                string firstLine = reader.ReadLine();

                const string START_TAG = "-----BEGIN PKCS7-----";
                const string END_TAG = "-----END PKCS7-----";

                bool isBase64Format = (firstLine.IndexOf(START_TAG) > -1);

                if (isBase64Format)
                {
                    string allContent = reader.ReadToEnd();
                    allContent = allContent.Replace(END_TAG, string.Empty);

                    char[] tmp = allContent.ToCharArray();
                    signedContent = Convert.FromBase64CharArray(tmp, 0, tmp.Length);
                }
                else
                    signedContent = content;
            }

            return signedContent;
        }

        /// <summary>
        /// Verify PKCS7 signature
        /// </summary>
        /// <returns>CAPICOM/CryptoAPI return code or an ApplicationException in file hash doesn't match</returns>
        public bool Verify(ref List<string> ErrorMessageLst)
        {
            bool rc = true;
            //DocsPaUtils.LogsManagement.Debugger.Write("SignedDocument.Verify - INIT");
            try
            {
                // Decodifica un messaggio SignedCms codificato. 
                // Al completamento della decodifica, è possibile recuperare le 
                // informazioni decodificate dalle proprietà dell'oggetto SignedCms.
                CmsSignedData cms = new CmsSignedData(this.GetSignedContent(this._buf));
                IX509Store store = cms.GetCertificates("Collection");
                SignerInformationStore signers = cms.GetSignerInfos();


                SignedData da = SignedData.GetInstance(cms.ContentInfo.Content.ToAsn1Object());
                
                Asn1Sequence DigAlgAsn1 = null;
                if (da.DigestAlgorithms.Count >0)
                    DigAlgAsn1 =da.DigestAlgorithms[0].ToAsn1Object() as Asn1Sequence;

                if (DigAlgAsn1 != null)
                    this._SignAlgorithm = Org.BouncyCastle.Security.DigestUtilities.GetAlgorithmName(AlgorithmIdentifier.GetInstance(DigAlgAsn1).ObjectID);
                
                
                //DocsPaUtils.LogsManagement.Debugger.Write("SignedDocument.Verify - Decode signed message");

                //  Verify signature. Do not validate signer
                //  certificate for the purposes of this example.
                //  Note that in a production environment, validating
                //  the signer certificate chain will probably
                //  be necessary.

                /*
                 * verifica le firme digitali nel messaggio CMS/PKCS #7 firmato e, 
                 * facoltativamente, convalida i certificati del firmatario.
                 *
                 * * Se verifySignatureOnly è true, vengono verificate solo le firme digitali. 
                 * Se è false, vengono verificate le firme digitali e vengono convalidati 
                 * i certificati dei firmatari e gli scopi dei certificati. 
                 * Gli scopi di un certificato sono considerati validi se il certificato 
                 * non prevede l'utilizzo della chiave o se l'utilizzo della chiave supporta 
                 * le firme digitali o il non-rifiuto.
                 * 
                */
                //_signersInfo = new DocsPaVO.documento.SignerInfo[signers.GetSigners().Count];
                //int i = 0;

                List<DocsPaVO.documento.SignerInfo> signInfoLst = new List<DocsPaVO.documento.SignerInfo>();
                foreach (SignerInformation signer in signers.GetSigners())
                {
                    DocsPaVO.documento.SignerInfo thisSinger = ExtractSignerInfo(ErrorMessageLst, store, signer);
                    signInfoLst.Add(thisSinger);
                }

                _signersInfo = signInfoLst.ToArray();
                //DocsPaUtils.LogsManagement.Debugger.Write(string.Format("SignedDocument.Verify - CheckSignature OK, signers count: {0}", signers.GetSigners().Count));
                CmsProcessable signedContent = cms.SignedContent;
                this._content = (byte[])signedContent.GetContent();

                if ((this._SignAlgorithm != null) && this._content != null)
                {
                    try
                    {
                        this._SignHash = BitConverter.ToString(Org.BouncyCastle.Security.DigestUtilities.CalculateDigest(this._SignAlgorithm, this._content)).Replace("-", "");
                    }
                    catch (Exception e)
                    {
                        ErrorMessageLst.Add(e.Message);
                    }
                }

                //DocsPaUtils.LogsManagement.Debugger.Write(string.Format("SignedDocument.Verify - Extact content, lenght: {0}", this._content.Length));

                this._documentFileName = GetDocumentFileName();
                this._SignType = "CADES";
                //DocsPaUtils.LogsManagement.Debugger.Write(string.Format("SignedDocument.Verify - DocumentFileName: '{0}'", this._documentFileName));
            }
            catch (Exception ex)
            {
                rc = false;
                //DocsPaUtils.LogsManagement.Debugger.Write("SignedDocument.Verify - Si è verificato un errore nella verifica della firma", ex);
                //throw new ApplicationException(ex.Message);
                ErrorMessageLst.Add(ex.Message);
            }
            finally
            {
                //DocsPaUtils.LogsManagement.Debugger.Write("SignedDocument.Verify - END");
            }

            return rc;
        }

        private DocsPaVO.documento.SignerInfo ExtractSignerInfo(List<string> ErrorMessageLst, IX509Store store, SignerInformation signer)
        {
            DocsPaVO.documento.SignerInfo thisSinger = new DocsPaVO.documento.SignerInfo();
            Org.BouncyCastle.X509.X509Certificate cert1 = GetCertificate(signer, store);
            try
            {
                if (!signer.Verify(cert1))
                    ErrorMessageLst.Add("Not valid signature");
            }
            catch (Exception e)
            {
                ErrorMessageLst.Add(e.Message);
            }
            
            thisSinger.isCountersigner = signer.IsCounterSignature;
            if (signer.SignedAttributes != null)
            {
                
                if (signer.SignedAttributes[Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.IdAASigningCertificateV2] == null)
                    ErrorMessageLst.Add("Id-AA-SigningCertificateV2 not found");

                if (signer.SignedAttributes[CmsAttributes.MessageDigest] == null)
                    ErrorMessageLst.Add("Pkcs9AtMessageDigest not found");


                if (!signer.IsCounterSignature) //Pare che i controfirmatari non ncessitino di questo parametro
                {
                    if (signer.SignedAttributes[Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.Pkcs9AtContentType] == null)
                        ErrorMessageLst.Add("Pkcs9AtContentType not found");
                }

                thisSinger.SignatureAlgorithm = Org.BouncyCastle.Security.DigestUtilities.GetAlgorithmName(signer.DigestAlgorithmID.ObjectID);
                if (signer.SignedAttributes[CmsAttributes.SigningTime]!=null)
                {
                    Org.BouncyCastle.Asn1.Cms.Attribute sigTime = signer.SignedAttributes[CmsAttributes.SigningTime];
                    if (sigTime.AttrValues.Count > 0)
                    {
                        try
                        {

                            thisSinger.SigningTime = GetSigningTime(sigTime.AttrValues[0]);
                        }
                        catch (Exception e)
                        {
                            ErrorMessageLst.Add("Error retriving SigningTime");
                        }

                    }
                }
            }
            else
            {
                ErrorMessageLst.Add("Missing SignedAttributes");
            }

            if (gestioneTSFirma)
            {
                List<TSInfo> tsArr = new List<TSInfo>();
                if (signer.UnsignedAttributes != null && signer.UnsignedAttributes[Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.IdAASignatureTimeStampToken] != null)
                {

                    ICollection ret = Org.BouncyCastle.Tsp.TspUtil.GetSignatureTimestamps(signer);
                    foreach (Org.BouncyCastle.Tsp.TimeStampToken token in ret)
                    {
                        VerifyTimeStamp verifyTimeStamp = new VerifyTimeStamp();
                        ICollection certsColl = token.GetCertificates("COLLECTION").GetMatches(null);
                        TSInfo timeStamp = verifyTimeStamp.getTSCertInfo(certsColl);

                        timeStamp.TSdateTime = token.TimeStampInfo.GenTime.ToLocalTime();
                        timeStamp.TSserialNumber = token.TimeStampInfo.SerialNumber.ToString();
                        timeStamp.TSimprint = Convert.ToBase64String(token.TimeStampInfo.TstInfo.MessageImprint.GetEncoded());
                        timeStamp.TSdateTime = token.TimeStampInfo.GenTime;
                        timeStamp.TSType = TsType.PKCS;
                        tsArr.Add(timeStamp);
                    }
                }

                if (tsArr.Count > 0)
                    thisSinger.SignatureTimeStampInfo = tsArr.ToArray();
            }
            X509Certificate2 cert = new X509Certificate2(cert1.GetEncoded());
            thisSinger.CertificateInfo.RevocationStatus = CheckCertificate(cert);
            thisSinger.CertificateInfo.X509Certificate = cert1.GetEncoded();

            thisSinger.CertificateInfo.RevocationStatusDescription = DecodeStatus(thisSinger.CertificateInfo.RevocationStatus);
            ParseCNIPASubjectInfo(ref thisSinger.SubjectInfo, cert.SubjectName.Name);

            thisSinger.CertificateInfo.IssuerName = cert.IssuerName.Name;
            thisSinger.CertificateInfo.SerialNumber = cert.SerialNumber;
            thisSinger.CertificateInfo.SignatureAlgorithm = cert.SignatureAlgorithm.FriendlyName;
            thisSinger.CertificateInfo.SubjectName = cert.SubjectName.Name;
            thisSinger.CertificateInfo.ValidFromDate = cert.NotBefore;
            thisSinger.CertificateInfo.ValidToDate = cert.NotAfter;
            thisSinger.CertificateInfo.ThumbPrint = cert.Thumbprint;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("CertificateInfo.IssuerName: '{0}'", thisSinger.CertificateInfo.IssuerName);
            sb.AppendFormat("CertificateInfo.SerialNumber: '{0}'", thisSinger.CertificateInfo.SerialNumber);
            sb.AppendFormat("CertificateInfo.SignatureAlgorithm: '{0}'", thisSinger.CertificateInfo.SignatureAlgorithm);
            sb.AppendFormat("CertificateInfo.SubjectName: '{0}'", thisSinger.CertificateInfo.SubjectName);
            sb.AppendFormat("CertificateInfo.ValidFromDate: '{0}'", thisSinger.CertificateInfo.ValidFromDate);
            sb.AppendFormat("CertificateInfo.ValidToDate: '{0}'", thisSinger.CertificateInfo.ValidToDate);
            sb.AppendFormat("CertificateInfo.ThumbPrint: '{0}'", thisSinger.CertificateInfo.ThumbPrint);
            //DocsPaUtils.LogsManagement.Debugger.Write(string.Format("SignedDocument.Verify - {0}", sb.ToString()));


            //gestione controfirma
            if (signer.UnsignedAttributes != null)
            {
                if (signer.UnsignedAttributes[CmsAttributes.CounterSignature] != null)
                {
                    List<DocsPaVO.documento.SignerInfo> cSigsList = new List<DocsPaVO.documento.SignerInfo>();
                    List<string> csignErrs = new List<string> ();
                    SignerInformationStore counterSignatures = signer.GetCounterSignatures();
                    foreach (SignerInformation conunterSig in counterSignatures.GetSigners())
                    {
                        DocsPaVO.documento.SignerInfo cSigs =ExtractSignerInfo(csignErrs, store, conunterSig);
                        cSigsList.Add(cSigs);
                    }
                    if (csignErrs.Count >0)
                        ErrorMessageLst.AddRange(csignErrs);
                    if (cSigsList.Count > 0)
                        thisSinger.counterSignatures = cSigsList.ToArray();
                }

            }
            return thisSinger;
        }

        //public List<DocsPaVO.documento.SignerInfo> getSILst(

        private string buildSubject(X509Name SubjectDN)
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


        static DateTime GetSigningTime(Asn1Encodable encodable)
        {
            // Special attention to the conversion from Der*Time to .Net's DateTime
            // (May lost timezone information)

            // Try to parse as UTC time
            try
            {
                DerUtcTime timeUtc = (DerUtcTime)DerUtcTime.GetInstance(encodable);
                return timeUtc.ToAdjustedDateTime();
            }
            catch (Exception e)
            {
            }

            // Try to parse as GeneralizedTime
            try
            {
                DerGeneralizedTime timeGenTime = (DerGeneralizedTime)DerGeneralizedTime.GetInstance(encodable);
                return timeGenTime.ToDateTime();
            }
            catch (Exception e)
            {
            }

            return DateTime.Now;
        }

        public DocsPaVO.documento.SignerInfo GetCertSignersInfo(byte [] cert)
        {
             X509CertificateParser cp = new X509CertificateParser();
             Org.BouncyCastle.X509.X509Certificate cert1 = cp.ReadCertificate(cert);
            return this.GetCertSignersInfo(cert1);
        }

        public  DocsPaVO.documento.SignerInfo GetCertSignersInfo(Org.BouncyCastle.X509.X509Certificate cert1)
        {
            DocsPaVO.documento.SignerInfo retval = new DocsPaVO.documento.SignerInfo();
            retval.SubjectInfo = new DocsPaVO.documento.SubjectInfo();
            string Subject = buildSubject(cert1.SubjectDN);

            ParseCNIPASubjectInfo(ref retval.SubjectInfo, Subject);
            retval.CertificateInfo.IssuerName = cert1.IssuerDN.ToString();
            retval.CertificateInfo.SerialNumber = BitConverter.ToString(cert1.SerialNumber.ToByteArray()).Replace("-", "");
            retval.CertificateInfo.SignatureAlgorithm = cert1.SigAlgName;
            retval.CertificateInfo.SubjectName = Subject;
            retval.CertificateInfo.ValidFromDate = cert1.NotBefore.ToLocalTime();
            retval.CertificateInfo.ValidToDate = cert1.NotAfter.ToLocalTime();
            retval.CertificateInfo.ThumbPrint = BitConverter.ToString(System.Security.Cryptography.SHA1.Create().ComputeHash(cert1.GetEncoded())).Replace("-", "");
            retval.CertificateInfo.X509Certificate = cert1.GetEncoded ();
            // controlla revoca
            X509Certificate2 cert = new X509Certificate2(cert1.GetEncoded());
            retval.CertificateInfo.RevocationStatus = CheckCertificate(cert);
            retval.CertificateInfo.RevocationStatusDescription = DecodeStatus(retval.CertificateInfo.RevocationStatus);

            return retval;
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

            /* ABBATANGELI - Codice precedente che ritorna dai errati
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
            */
            
            foreach (string strItem in items)
            {
                Regex regex = new Regex("[A-Za-z]*=[^\"][^,]*");
                MatchCollection matchColl = regex.Matches(strItem);

                for (int i = 0; i < matchColl.Count; i++)
                {
                    string item = matchColl[i].Value;

                    int indexOfValue = item.IndexOf("=");
                    string itemKey = item.Substring(0, indexOfValue).ToUpper().Trim();
                    string itemValue = item.Substring(indexOfValue + 1).ToUpper().Trim();
                    retValue[itemKey] = itemValue;
                }
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

        /// <summary>
        /// Parse Certificate Description
        /// </summary>
        /// <param name="sd">a structure filled with results</param>
        /// <param name="subject">the input string</param>
        public void ParseCNIPASubjectInfo(ref DocsPaVO.documento.SubjectInfo subjectInfo, string subject)
        {
            //pulizia
            if (subject.Contains(" + "))
                subject = subject.Replace(" + ", ", ");

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
                subjectInfo.SerialNumber = this.GetSubjectItem(subjectItems, "OID.2.5.4.5");
                if (string.IsNullOrEmpty ( subjectInfo.SerialNumber))
                    subjectInfo.SerialNumber = this.GetSubjectItem(subjectItems, "SERIALNUMBER");

                // Se il country code è "IT", il "SerialNumber" contiene il codice fiscale del titolare del certificato
                if (subjectInfo.SerialNumber.StartsWith("IT:"))
                    subjectInfo.CodiceFiscale = subjectInfo.SerialNumber.Substring(subjectInfo.SerialNumber.IndexOf(":") + 1);
                subjectInfo.Country = this.GetSubjectItem(subjectItems, "C");
                subjectInfo.Organizzazione = this.GetSubjectItem(subjectItems, "O");

                if (subjectInfo.Nome.StartsWith(", SERIALNUMBER="))
                    subjectInfo.Nome = subjectInfo.CommonName;

                if (subjectInfo.CertId.StartsWith(", SN="))
                    subjectInfo.CertId = string.Empty;


                subjectItems.Clear();
                subjectItems = null;
            }
        }

        private string GetSubjectItem(Hashtable subjectItems, string itemKey)
        {
            if (subjectItems.ContainsKey(itemKey))
                return subjectItems[itemKey].ToString();
            else
                return string.Empty;
        }

        /// <summary>
        /// Saves PKCS7 embedded document
        /// </summary>
        /// <param name="pathName">fileName to write</param>
        public void SaveContentToFile(string pathName)
        {
            if (_content != null)
            {
                FileStream fso = null;
                try
                {
                    fso = new FileStream(pathName, FileMode.Create, FileAccess.Write, FileShare.Write);
                    fso.Write(_content, 0, _content.Length);
                }
                catch (Exception e)
                {
                    Debug.Write(String.Format("Error writing file: {0}", e.Message));
                }
                finally
                {
                    if (fso != null) fso.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cert"></param>
        /// <returns></returns>
        private int CheckCertificate(X509Certificate2 cert)
        {
            int retValue = 0;
            X509Chain chain = new X509Chain();

            // check entire chain for revocation
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
            //DocsPaUtils.LogsManagement.Debugger.Write(string.Format("SignedDocument.CheckCertificate - RevocationFlag: {0}", chain.ChainPolicy.RevocationFlag));

            if (this._crlOnlineCheck)
                // check online and offline revocation lists
                chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
            else
                chain.ChainPolicy.RevocationMode = X509RevocationMode.Offline;

            //DocsPaUtils.LogsManagement.Debugger.Write(string.Format("SignedDocument.CheckCertificate - RevocationMode: {0}", chain.ChainPolicy.RevocationMode));

            // timeout for online revocation list
            chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 0, this._crlRetrieveTimeout);
            //DocsPaUtils.LogsManagement.Debugger.Write(string.Format("SignedDocument.CheckCertificate - UrlRetrievalTimeout: {0}", chain.ChainPolicy.UrlRetrievalTimeout));

            // no exceptions, check all properties
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
            //DocsPaUtils.LogsManagement.Debugger.Write(string.Format("SignedDocument.CheckCertificate - VerificationFlags: {0}", chain.ChainPolicy.VerificationFlags));

            // modify time of verification
            //chain.ChainPolicy.VerificationTime = new DateTime(1999, 1, 1);

            chain.Build(cert);

            if (chain.ChainStatus.Length != 0)
            {
                // Verifica lo stato del controllo del certificato
                // Nel caso siano rilevate errori nella verifica, vengono assegnate
                // le seguenti priorità:
                //  - UntrustedRoot
                //  - CtlNotTimeValid
                //  - CtlNotSignatureValid
                //  - RevocationStatusUnknown
                //  - Revoked

                retValue = this.CheckChainStatus(chain, X509ChainStatusFlags.NotTimeValid);
                if (retValue == 0)
                {
                    retValue = this.CheckChainStatus(chain, X509ChainStatusFlags.Revoked);
                }
                if (retValue == 0)
                {
                    retValue = this.CheckChainStatus(chain, X509ChainStatusFlags.NotSignatureValid);
                }
                if (retValue == 0)
                {
                    retValue = this.CheckChainStatus(chain, X509ChainStatusFlags.NotValidForUsage);
                }
                if (retValue == 0)
                {
                    retValue = this.CheckChainStatus(chain, X509ChainStatusFlags.UntrustedRoot);
                }
                if (retValue == 0)
                {
                    retValue = this.CheckChainStatus(chain, X509ChainStatusFlags.RevocationStatusUnknown);
                }
                if (retValue == 0)
                {
                    retValue = this.CheckChainStatus(chain, X509ChainStatusFlags.Cyclic);
                }
                if (retValue == 0)
                {
                    retValue = this.CheckChainStatus(chain, X509ChainStatusFlags.InvalidExtension);
                }
                if (retValue == 0)
                {
                    retValue = this.CheckChainStatus(chain, X509ChainStatusFlags.InvalidPolicyConstraints);
                }
                if (retValue == 0)
                {
                    retValue = this.CheckChainStatus(chain, X509ChainStatusFlags.InvalidBasicConstraints);
                }
                if (retValue == 0)
                {
                    retValue = this.CheckChainStatus(chain, X509ChainStatusFlags.InvalidNameConstraints);
                }
                if (retValue == 0)
                {
                    retValue = this.CheckChainStatus(chain, X509ChainStatusFlags.HasNotSupportedNameConstraint);
                }
                if (retValue == 0)
                {
                    retValue = this.CheckChainStatus(chain, X509ChainStatusFlags.HasNotDefinedNameConstraint);
                }
                if (retValue == 0)
                {
                    retValue = this.CheckChainStatus(chain, X509ChainStatusFlags.HasNotPermittedNameConstraint);
                }
                if (retValue == 0)
                {
                    retValue = this.CheckChainStatus(chain, X509ChainStatusFlags.HasExcludedNameConstraint);
                }
                if (retValue == 0)
                {
                    retValue = this.CheckChainStatus(chain, X509ChainStatusFlags.PartialChain);
                }
                if (retValue == 0)
                {
                    retValue = this.CheckChainStatus(chain, X509ChainStatusFlags.CtlNotTimeValid);
                }
                if (retValue == 0)
                {
                    retValue = this.CheckChainStatus(chain, X509ChainStatusFlags.CtlNotSignatureValid);
                }
                if (retValue == 0)
                {
                    retValue = this.CheckChainStatus(chain, X509ChainStatusFlags.CtlNotValidForUsage);
                }
                if (retValue == 0)
                {
                    retValue = this.CheckChainStatus(chain, X509ChainStatusFlags.OfflineRevocation);
                }
                if (retValue == 0)
                {
                    retValue = this.CheckChainStatus(chain, X509ChainStatusFlags.NoIssuanceChainPolicy);
                }

                //DocsPaUtils.LogsManagement.Debugger.Write(string.Format("SignedDocument.CheckCertificate - Status: '{0}', StatusDescription: '{1}'", retValue, this.DecodeStatus(retValue)));
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chain"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        private int CheckChainStatus(X509Chain chain, X509ChainStatusFlags flag)
        {
            X509ChainStatus status = chain.ChainStatus.Where(e => e.Status == flag).FirstOrDefault();

            return (int)status.Status;
        }

        /// <summary>
        /// converts numeric status code in human readable status string
        /// </summary>
        /// <param name="statusCode">input status code</param>
        /// <returns>status string</returns>
        public string DecodeStatus(int statusCode)
        {
            string statusString;
            switch (statusCode)
            {
                case 0:
                    statusString = "Valido";
                    break;

                case (int)X509ChainStatusFlags.NotTimeValid:
                    statusString = "La catena X509 non è valida a causa di un valore temporale non valido, ad esempio un valore che indica un certificato scaduto.";
                    break;

                case (int)X509ChainStatusFlags.Revoked:
                    statusString = "Il certificate è stato revocato in quanto presente nella CRL";
                    break;

                case (int)X509ChainStatusFlags.NotSignatureValid:
                    statusString = "La catena X509 non è valida a causa di una firma di certificato non valida.";
                    break;

                case (int)X509ChainStatusFlags.NotValidForUsage:
                    statusString = "L'utilizzo della chiave non è valido.";
                    break;

                case (int)X509ChainStatusFlags.UntrustedRoot:
                    statusString = "Il certificato della root authority che ha emesso il certificato in esame non è presente nel SYSTEM store della macchina che esegue il servizio di verifica";
                    break;

                case (int)X509ChainStatusFlags.RevocationStatusUnknown:
                    statusString = "Non è stato possibile verificare lo stato di revoca del certificato (nella maggior parte dei casi questo si verifica quando il sistema non riesce a contattare via internet il CDP per scaricare una CRL valida)";
                    break;

                case (int)X509ChainStatusFlags.Cyclic:
                    statusString = "Non è stato possibile compilare la catena X509.";
                    break;

                case (int)X509ChainStatusFlags.InvalidExtension:
                    statusString = "La catena X509 non è valida a causa di un'estensione non valida.";
                    break;

                case (int)X509ChainStatusFlags.InvalidPolicyConstraints:
                    statusString = "La catena X509 non è valida a causa di vincoli di criteri non validi.";
                    break;

                case (int)X509ChainStatusFlags.InvalidBasicConstraints:
                    statusString = "La catena X509 non è valida a causa di vincoli di base non validi.";
                    break;

                case (int)X509ChainStatusFlags.InvalidNameConstraints:
                    statusString = "La catena X509 non è valida a causa di vincoli di nome non validi.";
                    break;

                case (int)X509ChainStatusFlags.HasNotSupportedNameConstraint:
                    statusString = "Il certificato non presenta un vincolo di nome supportato o che presenta un vincolo di nome non supportato.";
                    break;

                case (int)X509ChainStatusFlags.HasNotDefinedNameConstraint:
                    statusString = "Il certificato presenta un vincolo di nome non definito.";
                    break;

                case (int)X509ChainStatusFlags.HasNotPermittedNameConstraint:
                    statusString = "Il certificato presenta un vincolo di nome non consentito.";
                    break;

                case (int)X509ChainStatusFlags.HasExcludedNameConstraint:
                    statusString = "La catena X509 non è valida perché un certificato ha escluso un vincolo di nome.";
                    break;

                case (int)X509ChainStatusFlags.PartialChain:
                    statusString = "La catena X509 non può essere compilata fino al certificato radice.";
                    break;

                case (int)X509ChainStatusFlags.CtlNotTimeValid:
                    statusString = "Il certificato è scaduto o non ancora valido.";
                    break;

                case (int)X509ChainStatusFlags.CtlNotSignatureValid:
                    statusString = "La signature del certificato non è valida.";
                    break;

                case (int)X509ChainStatusFlags.CtlNotValidForUsage:
                    statusString = "L'elenco dei certificati attendibili non è valido per questo utilizzo.";
                    break;

                case (int)X509ChainStatusFlags.OfflineRevocation:
                    statusString = "L'elenco certificati revocati (CRL, Certificate Revocation List) online su cui si basa la catena X509 non è al momento online.";
                    break;

                case (int)X509ChainStatusFlags.NoIssuanceChainPolicy:
                    statusString = "Nel certificato non esiste alcuna estensione dei criteri di certificato. Questo errore si verifica se i criteri di gruppo specificano che tutti i certificati devono presentare criteri di certificato.";
                    break;

                default:
                    statusString = "STATUS_DESCRIPTION_UNKNOWN";
                    break;
            }
            return statusString;
        }


        /// <summary>
        /// reperimento descrizione algoritmo di firma
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        private string GetDescriptionAlgorithm(string oid)
        {
            if (this._algHT.ContainsKey(oid))
                return (string)this._algHT[oid];
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

        /// <summary>
        /// Caricamento hashtable contenente la mappatura della
        /// lista di algoritmi RSA disponibili
        /// </summary>
        private void FillTableAlgorithm()
        {
            if (_algHT.Count > 0)
                this._algHT.Clear();

            this._algHT.Add("1.2.840.113549", "RSA");
            this._algHT.Add("1.2.840.113549.1.1.1", "RSA_RSA");
            this._algHT.Add("1.2.840.113549.1.1.2", "RSA_MD2");
            this._algHT.Add("1.2.840.113549.1.1.3", "RSA_MD4");
            this._algHT.Add("1.2.840.113549.1.1.4", "RSA_MD5RSA");
            this._algHT.Add("1.2.840.113549.1.1.5", "RSA_SHA-1RSA");
            this._algHT.Add("1.2.840.113549.1.1.11", "SHA256RSA");
            this._algHT.Add("1.2.840.113549.1.1.12", "SHA384RSA");
            this._algHT.Add("1.2.840.113549.1.1.13", "SHA512RSA");
            this._algHT.Add("1.2.840.113549.1.1.10", "RSASSA-PSS");
            this._algHT.Add("1.2.840.113549.1.3.1", "RSA_DH");
            this._algHT.Add("1.2.840.113549.1.7.1", "RSA_data");
            this._algHT.Add("1.2.840.113549.1.7.2", "RSA_signedData");
            this._algHT.Add("1.2.840.113549.1.7.3", "RSA_envelopedData");
            this._algHT.Add("1.2.840.113549.1.7.4", "RSA_signEnvData");
            this._algHT.Add("1.2.840.113549.1.7.5", "RSA_digestedData");
            this._algHT.Add("1.2.840.113549.1.7.6", "RSA_encryptedData");
            this._algHT.Add("1.2.840.113549.1.9.1", "RSA_emailAddr");
            this._algHT.Add("1.2.840.113549.1.9.2", "RSA_unstructName");
            this._algHT.Add("1.2.840.113549.1.9.3", "RSA_contentType");
            this._algHT.Add("1.2.840.113549.1.9.4", "RSA_messageDigest");
            this._algHT.Add("1.2.840.113549.1.9.5", "RSA_signingTime");
            this._algHT.Add("1.2.840.113549.1.9.6", "RSA_counterSign");
            this._algHT.Add("1.2.840.113549.1.9.7", "RSA_challengePwd");
            this._algHT.Add("1.2.840.113549.1.9.8", "RSA_unstructAddr");
            this._algHT.Add("1.2.840.113549.1.9.9", "RSA_extCertAttrs");
            this._algHT.Add("1.2.840.113549.1.9.15", "RSA_SMIMECapabilities");
            this._algHT.Add("1.2.840.113549.1.9.15.1", "RSA_preferSignedData");
            this._algHT.Add("1.2.840.113549.2", "RSA_HASH");
            this._algHT.Add("1.2.840.113549.2.5", "RSA_MD5");
            this._algHT.Add("1.2.840.113549.3", "RSA_ENCRYPT");
            this._algHT.Add("1.2.840.113549.3.2", "RSA_RC2CBC");
            this._algHT.Add("1.2.840.113549.3.4", "RSA_RC4");
            this._algHT.Add("1.2.840.113549.3.7", "RSA_DES_EDE3_CBC");
            this._algHT.Add("1.2.840.113549.3.9", "RSA_RC5_CBCPad");
            this._algHT.Add("1.2.840.10045.4.1", "SHA1ECDSA");
            this._algHT.Add("1.2.840.10045.4.3.2", "SHA256ECDSA");
            this._algHT.Add("1.2.840.10045.4.3.3", "SHA384ECDSA");
            this._algHT.Add("1.2.840.10045.4.3.4", "sSHA512ECDSA");
            this._algHT.Add("1.2.840.10045.4.3","specifiedECDSA");
            this._algHT.Add("1.3.14.3.2.26", "SHA1");
            this._algHT.Add("1.3.14.3.2.27", "DSASHA1");
            this._algHT.Add("2.16.840.1.101.3.4.2.1", "SHA256");
            this._algHT.Add("2.16.840.1.101.3.4.2.2", "SHA384");
            this._algHT.Add("2.16.840.1.101.3.4.2.3", "SHA512");

        }

        public DocsPaVO.documento.SignerInfo ExtractSignerInfo(X509Certificate2 cert)
        {
            DocsPaVO.documento.SignerInfo signerInfo = new DocsPaVO.documento.SignerInfo();

            signerInfo.CertificateInfo.RevocationStatus = CheckCertificate(cert);
            //signerInfo.CertificateInfo.X509Certificate = cert1.GetEncoded();

            signerInfo.CertificateInfo.RevocationStatusDescription = DecodeStatus(signerInfo.CertificateInfo.RevocationStatus);
            ParseCNIPASubjectInfo(ref signerInfo.SubjectInfo, cert.SubjectName.Name);

            signerInfo.CertificateInfo.IssuerName = cert.IssuerName.Name;
            signerInfo.CertificateInfo.SerialNumber = cert.SerialNumber;
            signerInfo.CertificateInfo.SignatureAlgorithm = cert.SignatureAlgorithm.FriendlyName;
            signerInfo.CertificateInfo.SubjectName = cert.SubjectName.Name;
            signerInfo.CertificateInfo.ValidFromDate = cert.NotBefore;
            signerInfo.CertificateInfo.ValidToDate = cert.NotAfter;
            signerInfo.CertificateInfo.ThumbPrint = cert.Thumbprint;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("CertificateInfo.IssuerName: '{0}'", signerInfo.CertificateInfo.IssuerName);
            sb.AppendFormat("CertificateInfo.SerialNumber: '{0}'", signerInfo.CertificateInfo.SerialNumber);
            sb.AppendFormat("CertificateInfo.SignatureAlgorithm: '{0}'", signerInfo.CertificateInfo.SignatureAlgorithm);
            sb.AppendFormat("CertificateInfo.SubjectName: '{0}'", signerInfo.CertificateInfo.SubjectName);
            sb.AppendFormat("CertificateInfo.ValidFromDate: '{0}'", signerInfo.CertificateInfo.ValidFromDate);
            sb.AppendFormat("CertificateInfo.ValidToDate: '{0}'", signerInfo.CertificateInfo.ValidToDate);
            sb.AppendFormat("CertificateInfo.ThumbPrint: '{0}'", signerInfo.CertificateInfo.ThumbPrint);

            return signerInfo;
        }
    }
}
