using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.BouncyCastle.X509.Store;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1;
using System.Collections;
using System.Security.Cryptography;

namespace BusinessLogic.Documenti.DigitalSignature.PKCS_Utils
{
	public class Pkcs
	{
        /// <summary>
        /// Adds or removes Content to P7mFile
        /// </summary>
        /// <param name="CmsData">Signature Info</param>
        /// <param name="File">File, if null returns only Signature Info</param>
        /// <returns></returns>
        public static byte[] EmbedFileToPkcs(byte[] CmsData, byte[] File)
        {
            DerObjectIdentifier contentTypeOID = CmsObjectIdentifiers.Data;
            Asn1EncodableVector digestAlgs = new Asn1EncodableVector();
            Asn1EncodableVector signerInfos = new Asn1EncodableVector();
            Asn1OctetString octs = null;

            if (File != null)
                octs = new DerOctetString(File);
            Org.BouncyCastle.Asn1.Cms.ContentInfo encInfo = new Org.BouncyCastle.Asn1.Cms.ContentInfo(contentTypeOID, octs);

            CmsProcessable content = new CmsProcessableByteArray(CmsData);

            Asn1Set certificates = null;
            Asn1Set certrevlist = null;
            ArrayList _certs = new ArrayList();
            ArrayList _crls = new ArrayList();
            ICollection certsColl = new ArrayList();
            ICollection crlsColl = new ArrayList();

            CmsSignedData cms = new CmsSignedData(CmsData);
            SignerInformationStore signers = cms.GetSignerInfos();
            IX509Store store = cms.GetCertificates("Collection");
            IX509Store crls = cms.GetCrls("Collection");
            certsColl = store.GetMatches(null);
            crlsColl = crls.GetMatches(null);
            foreach (SignerInformation signer in signers.GetSigners())
            {
                //digestAlgs.Add(Helper.FixAlgID(signer.DigestAlgorithmID));
                digestAlgs.Add(signer.DigestAlgorithmID);
                signerInfos.Add(signer.ToSignerInfo());
            }
            foreach (Org.BouncyCastle.X509.X509Certificate cert in certsColl)
                _certs.Add(Asn1Object.FromByteArray(cert.GetEncoded()));

            foreach (Org.BouncyCastle.X509.X509Certificate clr in crlsColl)
                _crls.Add(Asn1Object.FromByteArray(clr.GetEncoded()));

            if (_certs.Count != 0)
                certificates = CreateBerSetFromList(_certs);

            if (_crls.Count != 0)
                certrevlist = CreateBerSetFromList(_crls);

            SignedData sd = new SignedData(
                new DerSet(digestAlgs),
                encInfo,
                certificates,
                certrevlist,
                new DerSet(signerInfos));

            Org.BouncyCastle.Asn1.Cms.ContentInfo contentInfo = new Org.BouncyCastle.Asn1.Cms.ContentInfo(CmsObjectIdentifiers.SignedData, sd);
            byte[] retval = new CmsSignedData(content, contentInfo.GetDerEncoded()).GetEncoded();
            string asn = BitConverter.ToString(retval).Replace("-", "");
            return retval;
        }





        public static byte[] getHashFromSignature(byte[] signature)
        {
            CmsSignedData content = new CmsSignedData(signature);
            
            byte[] retval = null;
            try
            {
                SignerInformationStore sistore = content.GetSignerInfos();
                foreach (SignerInformation signer in sistore.GetSigners())
                {
                    if (signer.SignedAttributes != null)
                    {
                        if (signer.SignedAttributes[Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.Pkcs9AtMessageDigest] != null)
                        {
                            Asn1Encodable enc = signer.SignedAttributes[Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.Pkcs9AtMessageDigest].AttrValues[0];
                            retval = Org.BouncyCastle.Asn1.Asn1OctetString.GetInstance(enc).GetOctets();
                        }
                    }
                    else
                    {
                        IX509Store store = content.GetCertificates("Collection");
                        ICollection certsColl = store.GetMatches(null);
                        foreach (Org.BouncyCastle.X509.X509Certificate cert in certsColl)
                        {
                            signer.Verify(cert);
                            retval = signer.GetContentDigest();
                            break;
                        }
                    }
                    if (retval != null)
                        return retval;
                }
            }
            catch
            {
                return getSha256(extractSignedContent(signature));
            }

            return null;
        }

        public static byte[] extractSignedContent(byte[] signedFile)
        {
            CmsSignedData content = new CmsSignedData(signedFile);
            CmsProcessable signedContent = content.SignedContent;
            return (byte[])signedContent.GetContent();
        }

        public static byte[] getSha256(byte[] content)
        {
            SHA256 mySHA256 = SHA256Managed.Create();
            return mySHA256.ComputeHash(content);
        }

        public static Asn1Set CreateBerSetFromList( IList berObjects)
        {
            Asn1EncodableVector v = new Asn1EncodableVector();

            foreach (Asn1Encodable ae in berObjects)
            {
                v.Add(ae);
            }

            return new BerSet(v);
        }
	}
}
