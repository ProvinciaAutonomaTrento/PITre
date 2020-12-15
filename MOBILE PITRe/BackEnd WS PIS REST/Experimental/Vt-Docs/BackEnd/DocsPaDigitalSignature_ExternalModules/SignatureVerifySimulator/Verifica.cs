using System.Xml;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using System.Collections.Generic;
using DocsPaVO.documento;

namespace SignatureVerify
{
    
 
   
    public class Verifica
    {


        public enum EsitoVerificaStatus
        {
            Valid = 0,         //OK
            NotTimeValid = 1,  //Scaduto
            Revoked = 4,       //Revocato
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
            public DocsPaVO.documento.VerifySignatureResult VerifySignatureResult;
            public byte[] content;
        } 

        private  byte[] readFile(string path)
        {
            return System.IO.File.ReadAllBytes(path);
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

        static string zeniDateConverter(DateTime? dataVerifica)
        {
            DateTime data= DateTime.MinValue;
            if  (dataVerifica.HasValue )
                data= dataVerifica.Value ;

            string retval = String.Format("{0}{1}{2}{3}{4}{5}",
                data.Year.ToString().Remove(0, 2).PadLeft(2, '0'),
                data.Month.ToString().PadLeft(2, '0'),
                data.Day.ToString().PadLeft(2, '0'),
                data.Hour.ToString().PadLeft(2, '0'),
                data.Minute.ToString().PadLeft(2, '0'),
                data.Second.ToString().PadLeft(2, '0'));
            return retval;
        }

        public EsitoVerifica VerificaByteEV(byte[] fileContents, string endPoint, Object[] args)
        {
            EsitoVerifica ev = new EsitoVerifica();
 
            string dataVerificaString = string.Empty;
            DateTime?  dataverificaDT;
            if (args.Length > 0)
            {
                dataverificaDT = args[0] as DateTime?;
                if (dataverificaDT == null)
                {
                    dataVerificaString = args[0] as string;
                    if (dataVerificaString == null)
                    {
                        ev.status =  EsitoVerificaStatus.ErroreGenerico;
                        return ev; 
                    }
                }
                else
                {
                    dataVerificaString = zeniDateConverter(dataverificaDT);
                }
            }

            ev.message = "Certificato del Firmatario revocato";
            ev.errorCode = "1408";
            ev.SubjectDN = "12202828:4330:1";
            ev.SubjectCN = "ELIO RAFFAELE OTTAVIANO";

            ev.status = EsitoVerificaStatus.ErroreGenerico ;
            ev.status = EsitoVerificaStatus.Revoked;
            ev.dataRevocaCertificato = DateTime.Parse("2011-01-21T12:48:17+01:00");
                

            //TEST!!!!

            ev.content =null;
            ev.VerifySignatureResult = ConvertToVerifySignatureResult(ev.status); ;
            return ev;

            
        }

        private static VerifySignatureResult ConvertToVerifySignatureResult(EsitoVerificaStatus status)
        {
            VerifySignatureResult vsr = new VerifySignatureResult();
            List<DocsPaVO.documento.SignerInfo> siLst = new List<DocsPaVO.documento.SignerInfo>();

            {
                DocsPaVO.documento.SignerInfo si = new DocsPaVO.documento.SignerInfo();
                si.CertificateInfo = new DocsPaVO.documento.CertificateInfo
                {
                    ValidFromDate = DateTime.Parse("2011-03-25T13:57:54+01:00"),
                    ValidToDate = DateTime.Parse("2014-03-25T00:00:00+01:00"),
                    RevocationStatus = (int)EsitoVerificaStatus.Valid,
                    RevocationStatusDescription = EsitoVerificaStatus.Valid.ToString()
                };

                si.SubjectInfo = new DocsPaVO.documento.SubjectInfo
                {
                    CodiceFiscale = "TTVLFF44P28D969E",
                    CommonName = "Elio Raffaele Ottaviano",
                    CertId = "2011500471127",
                };

                siLst.Add(si);
            }


            {
                DocsPaVO.documento.SignerInfo si = new DocsPaVO.documento.SignerInfo();
                si.CertificateInfo = new DocsPaVO.documento.CertificateInfo
                {
                    ValidFromDate = DateTime.Parse("2008-09-01T14:09:40+02:00"),
                    ValidToDate = DateTime.Parse("2013-12-31T18:00:00+01:00"),
                    RevocationDate = DateTime.Parse("2011-01-21T12:48:17+01:00"),
                    RevocationStatus = (int)EsitoVerificaStatus.Revoked,
                    RevocationStatusDescription = EsitoVerificaStatus.Revoked.ToString ()
                };

                si.SubjectInfo = new DocsPaVO.documento.SubjectInfo
                {
                    CodiceFiscale = "TTVLFF44P28D969E",
                    CommonName = "ELIO RAFFAELE OTTAVIANO",
                    CertId = "12202828:4330:1",
                };

                siLst.Add(si);
            }

            List<DocsPaVO.documento.PKCS7Document> p7docsLst = new List<DocsPaVO.documento.PKCS7Document>();
            DocsPaVO.documento.PKCS7Document p7doc = new DocsPaVO.documento.PKCS7Document
            {
                SignersInfo = siLst.ToArray(),
                DocumentFileName = null,
                Level = 0
            };
            p7docsLst.Add(p7doc);
            vsr.PKCS7Documents = p7docsLst.ToArray();
            vsr.StatusCode = (int)EsitoVerificaStatus.Revoked;
            vsr.StatusDescription = status.ToString();
            vsr.CRLOnlineCheck = true;
            return vsr;
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
