using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Tsp;

namespace BusinessLogic.Documenti.DigitalSignature.PKCS_Utils
{
    public class tsd : PKCS_Utils.ITimeStampedContainer
	{


        private List<CryptoFile> _tsr = new List<CryptoFile>();
        public List<CryptoFile> TSR
        {
            get { return _tsr; }
            set { _tsr = value; }
        }

        private CryptoFile _p7m;
        public CryptoFile Data
        {
            get { return _p7m; }
            set { _p7m = value; }
        }

        private CryptoFile _tsd;
        public CryptoFile cryptoFile
        {
            get { return _tsd; }
            set { _tsd = value; }
        }


        public tsd()
        {

        }

        public tsd(CryptoFile p7mFile, CryptoFile tsrFile)
        {

        }

        public tsd(CryptoFile TSDfile)
        {

        }
        public tsd(byte[] TSDFile)
        {

        }


        public void explode(byte[] fileContents)
        {
            try
            {
                Asn1Sequence sequenza = Asn1Sequence.GetInstance(fileContents);
                DerObjectIdentifier tsdOIDFile = sequenza[0] as DerObjectIdentifier;
                if (tsdOIDFile != null)
                {
                    if (tsdOIDFile.Id == CmsObjectIdentifiers.timestampedData.Id)   //TSD
                    {
                        DerTaggedObject taggedObject = sequenza[1] as DerTaggedObject;
                        if (taggedObject != null)
                        {
                            Asn1Sequence asn1seq = Asn1Sequence.GetInstance(taggedObject, true);
                            TimeStampedData tsd = TimeStampedData.GetInstance(asn1seq);
                            _p7m = new CryptoFile { Content =  tsd.Content.GetOctets(), MessageFileType =  fileType.Binary , Name = "default.p7m" };
                            TimeStampAndCrl[] crlTS = tsd.TemporalEvidence.TstEvidence.ToTimeStampAndCrlArray();
                            foreach (TimeStampAndCrl tokCRL in crlTS)
                            {
                                TimeStampToken tsToken = new TimeStampToken(tokCRL.TimeStampToken);
                                ContentInfo o = tokCRL.TimeStampToken;

                                Org.BouncyCastle.Asn1.Cmp.PkiStatusInfo si = new Org.BouncyCastle.Asn1.Cmp.PkiStatusInfo(0);
                                Org.BouncyCastle.Asn1.Tsp.TimeStampResp re = new Org.BouncyCastle.Asn1.Tsp.TimeStampResp(si, o);

                                string serial =tsToken.TimeStampInfo.SerialNumber.ToString();
                               _tsr.Add(new CryptoFile { Content = re.GetEncoded(), Name = String.Format("default.{0}.tsr", serial), MessageFileType = fileType.Binary });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public CryptoFile create(CryptoFile p7mFile, CryptoFile[] tsrFiles)
        {
            List<TimeStampAndCrl> tsCRLLst = new List<TimeStampAndCrl>();
            Asn1OctetString p7mOctecString = (Asn1OctetString)new DerOctetString(p7mFile.Content).ToAsn1Object();
            foreach (CryptoFile tsrFile in tsrFiles)
            {
                TimeStampResponse tsr = new TimeStampResponse(tsrFile.Content);
                TimeStampAndCrl tsCRL = new TimeStampAndCrl(tsr.TimeStampToken.ToCmsSignedData().ContentInfo);
                tsCRLLst.Add(tsCRL);
            }
            Evidence ev = new Evidence(new TimeStampTokenEvidence(tsCRLLst.ToArray()));
            TimeStampedData newTSD = new TimeStampedData(null, null, p7mOctecString, ev);
            ContentInfo info = new ContentInfo(CmsObjectIdentifiers.timestampedData, newTSD.ToAsn1Object());
            CryptoFile retval = new CryptoFile { Content = info.GetDerEncoded(), Name = "default.tsd", MessageFileType = fileType.Binary };
            return retval;
        }
	}
}
