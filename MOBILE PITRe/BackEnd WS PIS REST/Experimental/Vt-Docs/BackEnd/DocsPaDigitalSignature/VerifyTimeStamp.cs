using System;
using System.Collections;
using System.Text;
using System.Linq;
using System.Configuration;
using DocsPaVO.areaConservazione;
using System.IO;

using Org.BouncyCastle.Tsp;
using Org.BouncyCastle.Math;
using System.Security.Cryptography;
using DocsPaDB;
using log4net;
using Org.BouncyCastle.Asn1;

namespace BusinessLogic.Documenti.DigitalSignature
{
	public class VerifyTimeStamp
	{
        private ILog logger = LogManager.GetLogger(typeof(VerifyTimeStamp));
        public VerifyTimeStamp()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tsReq"></param>
        /// <param name="tsRes"></param>
        /// <returns></returns>
        public OutputResponseMarca Verify(TimeStampRequest tsReq, TimeStampResponse tsRes)
        {
            return checkMarca(tsRes, tsReq);
        }

        /// <summary>
        /// verifica implicitamente la marca a partire dalle sole informazioni contenute nel file TSR
        /// </summary>
        /// <param name="fileTSR"></param>
        /// <returns></returns>
        public OutputResponseMarca Verify(byte[] fileTSR)
        {
            TimeStampRequest tsReq = null;
            TimeStampResponse tsRes = null;

            if (fileTSR.Length > 0)
            {
                tsRes = getTsRes(fileTSR);
            }
            else
            {
                logger.Debug("File TSR mancante oppure corrotto!");
                return null;
            }

            TimeStampRequestGenerator tsqGen = new TimeStampRequestGenerator();
            //quando impostato a true significa che nella risposta della TSA deve essere incluso il certificato della firma
            tsqGen.SetCertReq(true);
            //fra i parametri non metto il Nonce perchè questa è una verifica off-line!!!
            byte[] fileHash = tsRes.TimeStampToken.TimeStampInfo.TstInfo.MessageImprint.GetHashedMessage();
            tsReq = tsqGen.Generate(TspAlgorithms.Sha256, fileHash);

            return checkMarca(tsRes, tsReq);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filep7m"></param>
        /// <param name="fileTSR"></param>
        /// <returns></returns>
        public OutputResponseMarca Verify(byte[] filep7m, byte[] fileTSR)
        {
            TimeStampRequest tsReq = null;
            TimeStampResponse tsRes = null;
            if (fileTSR.Length > 0)
            {
                tsRes = getTsRes(fileTSR);
            }
            else
            {
                logger.Debug("File TSR mancante oppure corrotto!");
                return null;
            }
            DerObjectIdentifier algoOid = tsRes.TimeStampToken.TimeStampInfo.HashAlgorithm.ObjectID;
            if (filep7m.Length > 0)
            {
                tsReq = getTsReq(filep7m, algoOid);
            }
            else
            {
                logger.Debug("File p7m mancante oppure corrotto!");
                return null;
            }
            return checkMarca(tsRes, tsReq);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="t"></typeparam>
        /// <param name="propName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        static private object getProps(string propName, object obj)
        {
            try
            {
                Object retval = obj.GetType().GetProperty(propName).GetValue(obj, null);
                return retval;
            }
            catch { }
            return null;
        }

        public bool machTSR(byte[] TSR, byte[] fileToMatch)
        {
            TimeStampResponse resp = getTsRes(TSR);
            string algorithm = resp.TimeStampToken.TimeStampInfo.HashAlgorithm.ObjectID.Id;
            byte[] contentHash = Org.BouncyCastle.Security.DigestUtilities.CalculateDigest(algorithm, fileToMatch);
            byte[] tsrHash = resp.TimeStampToken.TimeStampInfo.TstInfo.MessageImprint.GetHashedMessage();
            return contentHash.SequenceEqual(tsrHash);
        }

        public DocsPaVO.documento.TSInfo getTSCertInfo(string base64TSR)
        {
            TimeStampResponse resp =  getTsRes(Convert.FromBase64String(base64TSR));

            ICollection certsColl = resp.TimeStampToken.GetCertificates("COLLECTION").GetMatches(null);
            DocsPaVO.documento.TSInfo retval = getTSCertInfo(certsColl);
            retval.TSdateTime = resp.TimeStampToken.TimeStampInfo.GenTime.ToLocalTime();
            retval.TSserialNumber = resp.TimeStampToken.TimeStampInfo.SerialNumber.ToString();
            retval.TSimprint = Convert.ToBase64String(resp.TimeStampToken.TimeStampInfo.TstInfo.MessageImprint.GetEncoded());
            retval.TSdateTime = resp.TimeStampToken.TimeStampInfo.GenTime;
            retval.TSType = DocsPaVO.documento.TsType.TSR;
            return retval;
        }

        /// <summary>
        /// Estrae determinati dati del certificato dal timestamp
        /// </summary>
        /// <param name="certsColl"></param>
        /// <returns></returns>
        public DocsPaVO.documento.TSInfo getTSCertInfo(ICollection certsColl)
        {
            DocsPaVO.documento.TSInfo retval = new DocsPaVO.documento.TSInfo();
           
            try
            {
                foreach (object obj in certsColl)
                {
                    retval.dataInizioValiditaCert = (DateTime)getProps("NotBefore", obj);
                    retval.dataFineValiditaCert = (DateTime)getProps("NotAfter", obj);

                    string issuer = (string)getProps("IssuerDN", obj).ToString();
                    string subject = (string)getProps("SubjectDN", obj).ToString();

                    DocsPaVO.documento.SubjectInfo subjectInfo = new DocsPaVO.documento.SubjectInfo();
                    SignedDocument sd = new SignedDocument();
                    sd.ParseCNIPASubjectInfo(ref subjectInfo, issuer);
                    retval.TSANameIssuer = subjectInfo.CommonName;

                    sd.ParseCNIPASubjectInfo(ref subjectInfo, subject);
                    retval.TSANameSubject = subjectInfo.CommonName;
                    break; //solo il primo
                }
            } catch {}
            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filep7m"></param>
        /// <param name="fileTSR"></param>
        /// <returns></returns>
        public OutputResponseMarca Verify(string filep7m, string fileTSR)
        {
            byte[] bytep7m;
            byte[] byteTSR;

            if (File.Exists(filep7m))
            {
                bytep7m = loadFile(filep7m);
            }
            else
            {
                logger.Debug("File p7m mancante oppure corrotto!");
                return null;
            }
            if (File.Exists(fileTSR))
            {
                byteTSR = loadFile(fileTSR);
            }
            else
            {
                logger.Debug("File TSR mancante oppure corrotto!");
                return null;
            }

            return Verify(bytep7m, byteTSR);
        }

        /// <summary>
        /// Questo metodo verifica se l'associazione fra marca e file è valida, verifica inoltre la
        /// validità del certificato firmatario della marca e la data di scadenza della marca; infine 
        /// restituisce (se le verifiche vanno a buon fine) tutti i dati contenuti nella marca.
        /// </summary>
        /// <param name="tsRes"></param>
        /// <param name="tsReq"></param>
        /// <returns></returns>
        protected OutputResponseMarca checkMarca(TimeStampResponse tsRes, TimeStampRequest tsReq)
        {
            OutputResponseMarca outTSR = new OutputResponseMarca();
            try
            {
                tsRes.Validate(tsReq);
                outTSR.esito = "OK";
                outTSR.descrizioneErrore = string.Empty;
            }
            catch (TspException e)
            {
                outTSR.esito = "KO";
                outTSR.descrizioneErrore = "verifica della marca fallita: " + e.Message;
                logger.Debug("verifica della marca fallita: " + e.Message);
                //return outTSR;
            }

            TimeStampToken tsToken = tsRes.TimeStampToken;
            //Verifica data scadenza marca secondo l'ora locale
            Org.BouncyCastle.X509.Store.IX509Store store = tsToken.GetCertificates("Collection");
            Org.BouncyCastle.X509.X509Certificate cert = (Org.BouncyCastle.X509.X509Certificate)new ArrayList(store.GetMatches(tsToken.SignerID))[0];
            //se la data attuale è maggiore di quella di scadenza del certificato che ha firmato la marca
            //allora la marca è scaduta!!!
            if (DateTime.Now.CompareTo(cert.NotAfter.ToLocalTime()) > 0)
            {
                outTSR.esito = "KO";
                outTSR.descrizioneErrore = "marca temporale scaduta";
                logger.Debug("marca temporale scaduta");
                //return outTSR;
            }

            try
            {
                //estrazione delle informazioni dalla marca
                outTSR.dsm = cert.NotAfter.ToLocalTime().ToString();
                outTSR.sernum = tsToken.TimeStampInfo.SerialNumber.ToString();
                outTSR.fhash = byteArrayToHexa(tsToken.TimeStampInfo.TstInfo.MessageImprint.GetHashedMessage());
                outTSR.docm = tsToken.TimeStampInfo.TstInfo.GenTime.TimeString;
                outTSR.docm_date = tsToken.TimeStampInfo.GenTime.ToLocalTime().ToString();
                outTSR.marca = Convert.ToBase64String(tsRes.GetEncoded());
                outTSR.algCertificato = cert.SigAlgName;
                outTSR.fromDate = cert.NotBefore.ToLocalTime().ToString();
                outTSR.snCertificato = cert.SerialNumber.ToString();
                //Algoritmo hash utilizzato per l'impronta
                string algHashOid = tsToken.TimeStampInfo.MessageImprintAlgOid;
                if (!string.IsNullOrEmpty(algHashOid))
                {
                    System.Security.Cryptography.Oid oidHash = new System.Security.Cryptography.Oid(algHashOid);
                    outTSR.algHash = oidHash.FriendlyName;
                }
                
                outTSR.TSA = new TSARFC2253();

                //Con le TSA di test potrebbe non essere valorizzato l'oggetto TSA
                logger.Debug("Controllo TSA : " + tsToken.TimeStampInfo.Tsa);
                try
                {
                    if (tsToken.TimeStampInfo.Tsa != null)
                    {
                        string oid = string.Empty;
                        string oidValue = string.Empty;
                        logger.Debug("TagNo: " + tsToken.TimeStampInfo.Tsa.TagNo);
                        for (int n = 0; n < tsToken.TimeStampInfo.Tsa.TagNo; n++)
                        {
                            logger.Debug("Tag: " + n);
                            Org.BouncyCastle.Asn1.Asn1Sequence seq = (Org.BouncyCastle.Asn1.Asn1Sequence)tsToken.TimeStampInfo.Tsa.Name.ToAsn1Object();

                            //Obsoleto
                            //Org.BouncyCastle.Asn1.Asn1Object obj = (Org.BouncyCastle.Asn1.Asn1Object)seq.GetObjectAt(n);
                            Org.BouncyCastle.Asn1.Asn1Object obj = (Org.BouncyCastle.Asn1.Asn1Object)seq[n];

                            Org.BouncyCastle.Asn1.Asn1Set set1 = (Org.BouncyCastle.Asn1.Asn1Set)obj.ToAsn1Object();

                            //Obsoleto
                            //seq = (Org.BouncyCastle.Asn1.Asn1Sequence)set1.GetObjectAt(0);
                            //obj = (Org.BouncyCastle.Asn1.Asn1Object)seq.GetObjectAt(0);
                            seq = (Org.BouncyCastle.Asn1.Asn1Sequence)set1[0];
                            obj = (Org.BouncyCastle.Asn1.Asn1Object)seq[0];


                            oid = obj.ToString();

                            //Obsoleto
                            //obj = (Org.BouncyCastle.Asn1.Asn1Object)seq.GetObjectAt(1);
                            obj = (Org.BouncyCastle.Asn1.Asn1Object)seq[1];

                            oidValue = obj.ToString();
                            System.Security.Cryptography.Oid oid_obj = new System.Security.Cryptography.Oid(oid);
                            string friendly = oid_obj.FriendlyName;
                            logger.Debug("oid: " + oid + " friendly: " + friendly);
                            switch (friendly)
                            {
                                case "CN":
                                    outTSR.TSA.CN = oidValue;
                                    break;
                                case "OU":
                                    outTSR.TSA.OU = oidValue;
                                    break;
                                case "O":
                                    outTSR.TSA.O = oidValue;
                                    break;
                                case "C":
                                    outTSR.TSA.C = oidValue;
                                    break;
                            }
                        }
                        outTSR.TSA.TSARFC2253Name = "CN=" + outTSR.TSA.CN + ",OU=" + outTSR.TSA.OU + ",O=" + outTSR.TSA.O + ",C=" + outTSR.TSA.C;
                    }
                }
                catch (Exception e)
                {
                    logger.Debug("Eccezione controllo TSA : " + e.Message);
                }
                logger.Debug("Fine Controllo TSA");
            }
            catch (Exception eTsp)
            {
                outTSR.esito = "KO";
                outTSR.descrizioneErrore = "estrazione delle informazioni dalla marca fallita: " + eTsp.Message;
                logger.Debug("estrazione delle informazioni dalla marca fallita: " + eTsp.Message);
                //return outTSR;
            }

            //verifico l'esistenza del documento al quale è associata la marca temporale
            //Commentata perchè l'impronta del documento è ancora calcolata con SHA1 invece che SHA256
            //DocsPaDB.Query_DocsPAWS.Documenti documento = new DocsPaDB.Query_DocsPAWS.Documenti();
            //outTSR.timestampedDoc = documento.GetDocNumberByImpronta(outTSR.fhash);
            //if (string.IsNullOrEmpty(outTSR.timestampedDoc))
            //{
            //    outTSR.timestampedDoc = "Non esiste alcun documento associato alla marca temporale.";
            //}

            //costruisco l'oggetto rappresentante il contenuto in chiaro della marca
            outTSR.DecryptedTSR = new Marca();
            outTSR.DecryptedTSR.content = contentMarca(outTSR);
            outTSR.DecryptedTSR.contentType = "text/html"; //"application/x-html";
            outTSR.DecryptedTSR.length = outTSR.DecryptedTSR.content.Length;

            return outTSR;
        }

        /// <summary>
        /// metodo che costruisce un html contenente le informazioni in chiaro della marca temporale,
        /// tale html viene restituito sotto forma di array di byte.
        /// </summary>
        /// <param name="timestamp">timestamp formattato come data</param>
        /// <param name="hexHash">hash del file al quale è associata la marca, sotto forma di stringa esadecimale</param>
        /// <returns></returns>
        protected byte[] contentMarca(OutputResponseMarca resultMarca)
        {
            string marcaHtml = string.Empty;

            marcaHtml = "<HTML>" +
                    "<font face='Verdana' size=2>" +
                    "<HEAD>" +
                    "<TITLE> Marca temporale </TITLE>" +
                    "</HEAD>" +
                    "<body>" +
                        "<p align='center'><b>Marca Temporale</b><br /></p>" +
                        "<table frame='border' style='border-style: double; border-width: inherit; width:90%; font-family: Verdana; font-size: 11pt;'>" +
                            "<tr>" +
                                "<td width='40%'>" +
                                    "Verifica:</td>" +
                                "<td>" +
                                    resultMarca.esito + "&nbsp;</td>" +
                            "</tr>" +
                            "<tr>" +
                                "<td>" +
                                    "Num. serie:</td>" +
                                "<td>" +
                                    resultMarca.sernum + "&nbsp;</td>" +
                            "</tr>" +
                            "<tr>" +
                                "<td>" +
                                    "Data ed ora (Locale):</td>" +
                                "<td>" +
                                    resultMarca.docm_date + "&nbsp;</td>" +
                            "</tr>" +
                            "<tr>" +
                                "<td>" +
                                    "Algoritmo certificato:</td>" +
                                "<td>" +
                                    resultMarca.algCertificato + "&nbsp;</td>" +
                            "</tr>" +
                            "<tr>" +
                                "<td>" +
                                    "S.N. certificato:</td>" +
                                "<td>" +
                                    resultMarca.snCertificato + "&nbsp;</td>" +
                            "</tr>" +
                            "<tr>" +
                                "<td>" +
                                    "Valido dal:</td>" +
                                "<td>" +
                                    resultMarca.fromDate + "&nbsp;</td>" +
                            "</tr>" +
                            "<tr>" +
                                "<td>" +
                                    "Valido sino al:</td>" +
                                "<td>" +
                                    resultMarca.dsm + "&nbsp;</td>" +
                            "</tr>" +
                            "<tr>" +
                                "<td>" +
                                    "Algoritmo di hash:</td>" +
                                "<td>" +
                                    resultMarca.algHash + "&nbsp;</td>" +
                            "</tr>" +
                            "<tr>" +
                                "<td>" +
                                    "Impronta del documento associato:</td>" +
                                "<td>" +
                                    resultMarca.fhash + "&nbsp;</td>" +
                            "</tr>" +
                            "<tr>" +
                                "<td>" +
                                    "Id del documento associato:</td>" +
                                "<td>" +
                                    resultMarca.timestampedDoc + "&nbsp;</td>" +
                            "</tr>" +
                            "<tr>" +
                                "<td>" +
                                    "Soggetto:</td>" +
                                "<td>" +
                                    resultMarca.TSA.O + "&nbsp;</td>" +
                            "</tr>" +
                            "<tr>" +
                                "<td>" +
                                    "Paese:</td>" +
                                "<td>" +
                                    resultMarca.TSA.C + "&nbsp;</td>" +
                            "</tr>" +
                        "</table>" +
                    "</body>" +
                    "</font>" +
                    "</html>";

            return System.Text.ASCIIEncoding.ASCII.GetBytes(marcaHtml);
        }

        #region utility
        /// <summary>
        /// Converte un file in una stringa Esadicimale
        /// </summary>
        /// <param name="byte8In"></param>
        /// <returns></returns>
        protected static string byteArrayToHexa(byte[] byte8In)
        {
            string retval = "";
            foreach (byte b in byte8In)
            {
                retval += String.Format("{0:X2}", b);
            }
            return retval;
        }

        /// <summary>
        /// Convert hex string to byte_array
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        protected byte[] String_To_Bytes(string strInput)
        {
            // i variable used to hold position in string
            int i = 0;
            // x variable used to hold byte array element position
            int x = 0;
            // allocate byte array based on half of string length
            byte[] bytes = new byte[(strInput.Length) / 2];
            // loop through the string - 2 bytes at a time converting
            //  it to decimal equivalent and store in byte array
            while (strInput.Length > i + 1)
            {
                long lngDecimal = Convert.ToInt32(strInput.Substring(i, 2), 16);
                bytes[x] = Convert.ToByte(lngDecimal);
                i = i + 2;
                ++x;
            }
            // return the finished byte array of decimal values
            return bytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        protected static byte[] loadFile(string fileName)
        {
            FileStream fs = File.OpenRead(fileName);
            BinaryReader br = new BinaryReader(fs);
            byte[] fileData = br.ReadBytes((int)fs.Length);
            br.Close();
            fs.Close();
            return fileData;
        }

        /// <summary>
        /// Ottiene un oggetto TimeStampRequest a partire dall'array di byte del documento da marcare
        /// </summary>
        /// <param name="filep7m"></param>
        /// <returns></returns>
        protected TimeStampRequest getTsReq(byte[] filep7m, DerObjectIdentifier oid)
        {
            TimeStampRequestGenerator tsqGen = new TimeStampRequestGenerator();
            //quando impostato a true significa che nella risposta della TSA deve essere incluso il certificato della firma
            tsqGen.SetCertReq(true);
            byte[] contentHash=null;
           
            //string algorithm = Org.BouncyCastle.Security.DigestUtilities.GetAlgorithmName(oid);
            string algorithm = oid.Id.ToString();
            contentHash = Org.BouncyCastle.Security.DigestUtilities.CalculateDigest(algorithm, filep7m);
            return tsqGen.Generate(algorithm, contentHash);


            /*
            
            TimeStampRequest tsq = null;

            if (hashOid.Equals("1.3.14.3.2.26"))
            {
                System.Security.Cryptography.SHA1 sh1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
                contentHash = sh1.ComputeHash(filep7m);
                tsq = tsqGen.Generate(TspAlgorithms.Sha1 , contentHash);
            }
            if (hashOid.Equals("2.16.840.1.101.3.4.2.1"))
            {
                SHA256Managed sh256 = new SHA256Managed();
                contentHash = sh256.ComputeHash(filep7m);
                tsq = tsqGen.Generate(TspAlgorithms.Sha256, contentHash);
            }
            
            //fra i parametri non metto il Nonce perchè questa è una verifica off-line!!!
            //TimeStampRequest tsq = tsqGen.Generate(TspAlgorithms.Sha1, contentHash);
            
            return tsq;
             */
        }

        /// <summary>
        /// Ottiene un oggetto TimeStampResponse a partire dall'array di byte del file TSR contenente la marca
        /// </summary>
        /// <param name="fileTSR"></param>
        /// <returns></returns>
        protected TimeStampResponse getTsRes(byte[] fileTSR)
        {
            return new TimeStampResponse(fileTSR);
        }
        #endregion
    }
}
