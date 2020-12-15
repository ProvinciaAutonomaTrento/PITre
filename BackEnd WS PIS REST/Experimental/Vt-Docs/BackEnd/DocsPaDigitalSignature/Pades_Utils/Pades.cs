using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using dpaItextSharp.text.pdf;
using System.Xml;
using dpaItextSharp.text;
using System.Security.Cryptography;
using Org.BouncyCastle.Asn1.Ess;
using Org.BouncyCastle.Asn1;
using CryptoUpgNet.NonExportablePK;
using Org.BouncyCastle.Security;
using System.Collections;
using Org.BouncyCastle.Cms;
using System.Security.Cryptography.Pkcs;

namespace BusinessLogic.Documenti.DigitalSignature.Pades_Utils
{
	public class Pades
	{

        /// <summary>
        /// Predisone un file Pades alla firma, se il parametro signature è vuoto torna l'hash del file da firmare, se è pieno lo firma
        /// </summary>
        /// <param name="data">i dati del PDF</param>
        /// <param name="signature">i dati della firma, se null calcola solo l'hash</param>
        /// <returns></returns>
        public static byte[] SignPadesFile(byte[] data,  byte[] signature)
        {
            MemoryStream outMs = new MemoryStream();
        
            PdfReader reader = new PdfReader(data);
            PdfStamper stp = null;
            bool isPades = true;
            //bool isPades = IsPdfPades(reader);
            bool isPdfA = IsPDFA(reader);

            if (isPades)  //se pades vado in append.
                stp = PdfStamper.CreateSignature(reader, outMs, '\0', null, true);
            else
                stp = PdfStamper.CreateSignature(reader, outMs, '\0');

            if (isPdfA)
                stp.Writer.PDFXConformance = PdfWriter.PDFA1A;

            
            PdfSignatureAppearance sap = stp.SignatureAppearance;

            if (isPdfA)
            {
                //BaseFont bf = BaseFont.CreateFont(@"c:\windows\fonts\arial.ttf", BaseFont.WINANSI, true);
                //forse va sistemato questo path.
                //Pades_Utils.dpaItextSharp.iTextSharp.text.pdf.fonts.Helvetica.afm
                //DPA.DigitalSignature.Itextsharp.iTextSharp.text.pdf.fonts.Helvetica.afm
                Stream fo = BaseFont.GetResourceStream("Pades_Utils.dpaItextSharp.iTextSharp.text.pdf.fonts.Helvetica.afm");
                byte[] fb = new BinaryReader(fo).ReadBytes((int)fo.Length);
                BaseFont bf = BaseFont.CreateFont("helvetica.afm", BaseFont.WINANSI, true, false, fb, fb);
                sap.Layer2Font = new Font(bf);
                //BaseFont bf1 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
                
                // bf = BaseFont.CreateFont (

                
            }

            PdfSignature dic = new PdfSignature(PdfName.ADOBE_PPKLITE, PdfName.ADBE_PKCS7_DETACHED);
            sap.CryptoDictionary = dic;

            //generazione nuovo pdf
            int csize = 10000;
            Dictionary<PdfName, int> exc = new Dictionary<PdfName, int>();
            exc[PdfName.CONTENTS] = csize * 2 + 2;
            Hashtable dic_hasht = new Hashtable(exc);
            sap.PreClose(dic_hasht);
            /*
            //tolgo l'id
            stp.Reader.Trailer.Put(dpaItextSharp.text.pdf.PdfName.ID, null);
            dpaItextSharp.text.pdf.PdfDictionary dict = (dpaItextSharp.text.pdf.PdfDictionary)stp.Reader.Trailer.GetAsDict(dpaItextSharp.text.pdf.PdfName.INFO);
            dict.Put(dpaItextSharp.text.pdf.PdfName.MODDATE, null);
            stp.Writer.Info.Put(dpaItextSharp.text.pdf.PdfName.MODDATE, null);
            */
            
            Stream s = sap.RangeStream;
            MemoryStream ss = new MemoryStream();
            int read = 0;
            byte[] buff = new byte[8192];
            while ((read = s.Read(buff, 0, 8192)) > 0)
                ss.Write(buff, 0, read);


            //se signature è vuota a me interessa SOLO l'hash sha256 e lo ritorno
            if (signature == null)
            {
                return PKCS_Utils.Pkcs.getSha256(ss.ToArray());
            }

            // ho una firma, procedo con l'append della firma sul file.
            byte[] outc = new byte[csize];

            PdfDictionary dic2 = new PdfDictionary();

            Array.Copy(signature, 0, outc, 0, signature.Length);

            dic2.Put(PdfName.CONTENTS, new PdfString(outc).SetHexWriting(true));
            sap.Close(dic2,true);
            outMs.Position = 0;
            BinaryReader br = new BinaryReader(outMs);
            byte[] retval = br.ReadBytes((int)outMs.Length);
            outMs.Close();
            return retval;

        }



        public byte[] SignDetached(byte[] data, int certIndex, string storeLocation, string storeName, string location, string reason, int position)
        {
            MemoryStream outMs = new MemoryStream();
           // X509Certificate2 card = GetCertificate(certIndex, storeLocation, storeName);
            // FAILLACE qui tocca fornire il certificato?
            X509Certificate2 card = null;

            Org.BouncyCastle.X509.X509CertificateParser cp = new Org.BouncyCastle.X509.X509CertificateParser();
            Org.BouncyCastle.X509.X509Certificate[] chain = new Org.BouncyCastle.X509.X509Certificate[] { cp.ReadCertificate(card.RawData) };

            PdfReader reader = new PdfReader(data);
            PdfStamper stp = null;
            bool isPades = IsPdfPades(reader);
            bool isPdfA = IsPDFA(reader);

            if (isPades)  //se pades vado in append.
                stp = PdfStamper.CreateSignature(reader, outMs, '\0', null, true);
            else
                stp = PdfStamper.CreateSignature(reader, outMs, '\0');

            if (isPdfA)
                stp.Writer.PDFXConformance = PdfWriter.PDFA1A;


            PdfSignatureAppearance sap = stp.SignatureAppearance;
            Rectangle pageSize = reader.GetPageSize(1);
            Rectangle signatureRect = setPosition(position, pageSize);
            sap.SetVisibleSignature(signatureRect, 1, null);
            sap.SignDate = DateTime.Now;
            sap.SetCrypto(null, chain, null, null);
            sap.Reason = reason;
            sap.Location = location;
            sap.Acro6Layers = true;
            sap.Render = PdfSignatureAppearance.SignatureRender.NameAndDescription;

            //così appare solo il testo che voglio io.
            sap.Layer2Text = "Test";
            sap.Render = PdfSignatureAppearance.SignatureRender.Description;

            if (isPdfA)
            {
                //BaseFont bf = BaseFont.CreateFont(@"c:\windows\fonts\arial.ttf", BaseFont.WINANSI, true);
                //forse va sistemato questo path.
                Stream fo = BaseFont.GetResourceStream("DPA.DigitalSignature.Itextsharp.iTextSharp.text.pdf.fonts.Helvetica.afm");

                byte[] fb = new BinaryReader(fo).ReadBytes((int)fo.Length);
                //BaseFont bf1 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, true);
                BaseFont bf = BaseFont.CreateFont("helvetica.afm", BaseFont.WINANSI, true, false, fb, fb);

                sap.Layer2Font = new Font(bf);
            }

            PdfSignature dic = new PdfSignature(PdfName.ADOBE_PPKLITE, PdfName.ADBE_PKCS7_DETACHED);
            dic.Date = new PdfDate(sap.SignDate);
            dic.Name = PdfPKCS7.GetSubjectFields(chain[0]).GetField("CN");
            if (sap.Reason != null)
                dic.Reason = sap.Reason;
            if (sap.Location != null)
                dic.Location = sap.Location;
            sap.CryptoDictionary = dic;
            int csize = 10000;
            Dictionary<PdfName, int> exc = new Dictionary<PdfName, int>();
            exc[PdfName.CONTENTS] = csize * 2 + 2;
            Hashtable dict_hasht = new Hashtable(exc);
            sap.PreClose(dict_hasht);

            Stream s = sap.RangeStream;
            MemoryStream ss = new MemoryStream();
            int read = 0;
            byte[] buff = new byte[8192];
            while ((read = s.Read(buff, 0, 8192)) > 0)
            {
                ss.Write(buff, 0, read);
            }

            byte[] pk = FirmaFileBouncy(ss.ToArray(), card);
            //pk = SignMsg(ss.ToArray(), card, true);

            byte[] outc = new byte[csize];

            PdfDictionary dic2 = new PdfDictionary();

            Array.Copy(pk, 0, outc, 0, pk.Length);

            dic2.Put(PdfName.CONTENTS, new PdfString(outc).SetHexWriting(true));
            sap.Close(dic2,true);
            outMs.Position = 0;
            BinaryReader br = new BinaryReader(outMs);
            byte[] retval = br.ReadBytes((int)outMs.Length);
            outMs.Close();
            return retval;
        }


        public Rectangle setPosition(int posi, Rectangle pagesize)
        {

            int height = 30;
            int len = 300;
            int startx = 5; //margine sinistro
            int starty = 5; //margine inferiore
            switch (posi)
            {


                case 0:// in alto a sinistra
                    {
                        starty = (int)pagesize.Height - starty - height;
                        return new Rectangle(startx, starty, startx + len, starty + height);
                    }
                case 1:// in alto a destra
                    {
                        starty = (int)pagesize.Height - starty - height;
                        startx = (int)pagesize.Width - startx - len;
                        return new Rectangle(startx, starty, startx + len, starty + height);
                    }
                case 2:// in basso a destra
                    {
                        startx = (int)pagesize.Width - startx - len;
                        return new Rectangle(startx, starty, startx + len, starty + height);

                    }
                default:// in basso a sinistra
                    {
                        return new Rectangle(startx, starty, startx + len, starty + height);
                    }


            }
            return null;

        }

        /// <summary>
        /// Ritorna true se in un file PDF sono presenti delle firme pades
        /// </summary>
        /// <param name="fileDoc"></param>
        /// <returns></returns>
        private static bool IsPdfPades(PdfReader r)
        {
            try
            {
                int numSig = 0;
                AcroFields af = r.AcroFields;
                if (af != null)
                {
                    numSig = af.GetSignatureNames().Count;
                    if (numSig > 0)
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Ritorna true se in un file PDF sono presenti delle firme pades
        /// </summary>
        /// <param name="fileDoc"></param>
        /// <returns></returns>
        public static bool IsPdfPades(DocsPaVO.documento.FileDocumento fileDoc)
        {
            try
            {
                int numSig = 0;
                dpaItextSharp.text.pdf.PdfReader r = new dpaItextSharp.text.pdf.PdfReader(fileDoc.content);
                dpaItextSharp.text.pdf.AcroFields af = r.AcroFields;
                if (af != null)
                {
                    numSig = af.GetSignatureNames().Count;
                    if (numSig > 0)
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }



        private static bool IsPDFA(PdfReader r)
        {
            bool retval = false;
            try
            {
                byte[] metadata = r.Metadata;
                if (metadata != null)
                {
                    try
                    {
                        string meta = System.Text.ASCIIEncoding.Default.GetString(metadata);
                        XmlDocument xdoc = new XmlDocument();
                        xdoc.LoadXml(meta);
                        XmlNodeList nodeList = xdoc.GetElementsByTagName("pdfaid:conformance");
                        if (nodeList.Item(0).FirstChild.Value.ToUpper() == "A")
                            retval = true;

                    }
                    catch { }
                }
            }
            catch { }
            return retval;
        }


        public static byte[] FirmaFileBouncy(byte[] data, X509Certificate2 cert)
        {
            try
            {
                SHA256Managed hashSha256 = new SHA256Managed();
                byte[] certHash = hashSha256.ComputeHash(cert.RawData);
                EssCertIDv2 essCert1 = new EssCertIDv2(new Org.BouncyCastle.Asn1.X509.AlgorithmIdentifier("2.16.840.1.101.3.4.2.1"), certHash);
                SigningCertificateV2 scv2 = new SigningCertificateV2(new EssCertIDv2[] { essCert1 });
                Org.BouncyCastle.Asn1.Cms.Attribute CertHAttribute = new Org.BouncyCastle.Asn1.Cms.Attribute(Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.IdAASigningCertificateV2, new DerSet(scv2));
                Asn1EncodableVector v = new Asn1EncodableVector();
                v.Add(CertHAttribute);
                Org.BouncyCastle.Asn1.Cms.AttributeTable AT = new Org.BouncyCastle.Asn1.Cms.AttributeTable(v);

                CmsSignedDataGenWithRsaCsp cms = new CmsSignedDataGenWithRsaCsp();

                var rsa = (RSACryptoServiceProvider)cert.PrivateKey;
                Org.BouncyCastle.X509.X509Certificate certCopy = DotNetUtilities.FromX509Certificate(cert);
                cms.MyAddSigner(rsa, certCopy, "1.2.840.113549.1.1.1", "2.16.840.1.101.3.4.2.1", AT, null);
                ArrayList certList = new ArrayList();
                certList.Add(certCopy);
                Org.BouncyCastle.X509.Store.X509CollectionStoreParameters PP = new Org.BouncyCastle.X509.Store.X509CollectionStoreParameters(certList);
                Org.BouncyCastle.X509.Store.IX509Store st1 = Org.BouncyCastle.X509.Store.X509StoreFactory.Create("CERTIFICATE/COLLECTION", PP);
                cms.AddCertificates(st1);
                //mi ricavo il file da firmare
                CmsSignedData Firmato = cms.Generate(new CmsProcessableByteArray(data), false);

                CmsSigner cmsSigner = new CmsSigner(cert);
                cmsSigner.IncludeOption = X509IncludeOption.EndCertOnly;

                System.Security.Cryptography.Pkcs.ContentInfo contentInfo = new System.Security.Cryptography.Pkcs.ContentInfo(Firmato.GetEncoded());
                SignedCms signedCms = new SignedCms();
                signedCms.Decode(Firmato.GetEncoded());
                byte[] ret = signedCms.Encode();
                return ret;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileDoc"></param>
        /// <returns></returns>
        public static bool VerifyPadesSignature(DocsPaVO.documento.FileDocumento fileDoc)
        {
            SignedDocument si = new SignedDocument();
            VerifyTimeStamp verifyTimeStamp = new VerifyTimeStamp();

            string padesSignAlgorithm = null;
            dpaItextSharp.text.pdf.PdfReader pdfReader = null;
            try
            {
                pdfReader = new dpaItextSharp.text.pdf.PdfReader(fileDoc.content);
            }
            catch
            {
                return false;
            }

            dpaItextSharp.text.pdf.AcroFields af = pdfReader.AcroFields;
            List<string> signNames = af.GetSignatureNames().Cast<string>().ToList<string>();

            if (signNames.Count == 0) //Firma non è presente
                return false;

            List<DocsPaVO.documento.SignerInfo> siList = new List<DocsPaVO.documento.SignerInfo>();
            bool verResult = true;


            foreach (string name in signNames)
            {
                List<DocsPaVO.documento.TSInfo> tsLst = new List<DocsPaVO.documento.TSInfo>();
                dpaItextSharp.text.pdf.PdfPKCS7 pk = af.VerifySignature(name);

                try
                {
                    padesSignAlgorithm = "PADES " + pk.GetHashAlgorithm();
                }
                catch (Exception exalg)
                {
                    padesSignAlgorithm = "PADES : errore ricavando l'algo Hash:" + exalg.Message;
                }

                byte[] cert = pk.SigningCertificate.GetEncoded();
                DocsPaVO.documento.SignerInfo sinfo = si.GetCertSignersInfo(cert);
                sinfo.SignatureAlgorithm = padesSignAlgorithm;
                sinfo.SigningTime = pk.SignDate;
                if (verResult) //fino a che è true verifica
                    verResult = pk.Verify();

                if (pk.TimeStampToken != null)
                {
                    //Ricavo il certificato
                    ICollection certsColl = pk.TimeStampToken.GetCertificates("COLLECTION").GetMatches(null);
                    DocsPaVO.documento.TSInfo timeStamp = verifyTimeStamp.getTSCertInfo(certsColl);

                    timeStamp.TSdateTime = pk.TimeStampToken.TimeStampInfo.GenTime.ToLocalTime();
                    timeStamp.TSserialNumber = pk.TimeStampToken.TimeStampInfo.SerialNumber.ToString();
                    timeStamp.TSimprint = Convert.ToBase64String(pk.TimeStampToken.TimeStampInfo.TstInfo.MessageImprint.GetEncoded());
                    timeStamp.TSType = DocsPaVO.documento.TsType.PADES;
                    tsLst.Add(timeStamp);
                }
                if (tsLst.Count > 0)
                    sinfo.SignatureTimeStampInfo = tsLst.ToArray();

                siList.Add(sinfo);
            }

            DocsPaVO.documento.VerifySignatureResult result = new DocsPaVO.documento.VerifySignatureResult();

            if (verResult)
            {
                result.StatusCode = 0;
                result.StatusDescription = "La Verifica OK, ma senza controllo CRL";
            }
            else
            {
                result.StatusCode = -1;
                result.StatusDescription = "La Verifica di almeno un firmatario e Fallita";
            }

            List<DocsPaVO.documento.PKCS7Document> pkcsDocs = new List<DocsPaVO.documento.PKCS7Document>();
            if ((fileDoc.signatureResult != null) && (fileDoc.signatureResult.PKCS7Documents != null) && (fileDoc.signatureResult.PKCS7Documents.Length > 0))
            {
                foreach (DocsPaVO.documento.PKCS7Document docs in fileDoc.signatureResult.PKCS7Documents)
                    pkcsDocs.Add(docs);
            }

            pkcsDocs.Add(new DocsPaVO.documento.PKCS7Document { SignersInfo = siList.ToArray(), SignAlgorithm = padesSignAlgorithm, DocumentFileName = fileDoc.nomeOriginale, SignHash = "Non Disponibile per la firma PADES", SignatureType = DocsPaVO.documento.SignType.PADES });
            result.PKCS7Documents = pkcsDocs.ToArray();
            result.FinalDocumentName = fileDoc.name;
            fileDoc.signatureResult = result;

            return false;
        }


	}


}
