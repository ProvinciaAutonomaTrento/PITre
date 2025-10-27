// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
extern alias BCrypto1;
extern alias BCrypto2;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using BCrypto1::Org.BouncyCastle.Cms;
using BCrypto1::Org.BouncyCastle.Asn1.Cms;
using BCrypto1::Org.BouncyCastle.Asn1;
using BCrypto1::Org.BouncyCastle.X509.Store;

namespace Pi3.App.Legacy.WebApi.Application.Sign
{
    internal static class Pkcs
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
            ContentInfo encInfo = new ContentInfo(contentTypeOID, octs);

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
            foreach (BCrypto1::Org.BouncyCastle.X509.X509Certificate cert in certsColl)
                _certs.Add(Asn1Object.FromByteArray(cert.GetEncoded()));

            foreach (BCrypto1::Org.BouncyCastle.X509.X509Certificate clr in crlsColl)
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

            ContentInfo contentInfo = new ContentInfo(CmsObjectIdentifiers.SignedData, sd);
            byte[] retval = new CmsSignedData(content, contentInfo.GetDerEncoded()).GetEncoded();
            string asn = BitConverter.ToString(retval).Replace("-", "");
            return retval;
        }

        public static byte[] getHashFromSignature(byte[] signature)
        {
            CmsSignedData content = new CmsSignedData(signature);

            byte[] retval = null!;
            try
            {
                SignerInformationStore sistore = content.GetSignerInfos();
                foreach (SignerInformation signer in sistore.GetSigners())
                {
                    if (signer.SignedAttributes != null)
                    {
                        if (signer.SignedAttributes[BCrypto1::Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.Pkcs9AtMessageDigest] != null)
                        {
                            Asn1Encodable enc = signer.SignedAttributes[BCrypto1::Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.Pkcs9AtMessageDigest].AttrValues[0];
                            retval = Asn1OctetString.GetInstance(enc).GetOctets();
                        }
                    }
                    else
                    {
                        IX509Store store = content.GetCertificates("Collection");
                        ICollection certsColl = store.GetMatches(null);

                        IEnumerator certEnumerator = certsColl.GetEnumerator();

                        if (certEnumerator.MoveNext())
                        {
                            var cert = (BCrypto1::Org.BouncyCastle.X509.X509Certificate)certEnumerator.Current;

                            signer.Verify(cert);
                            retval = signer.GetContentDigest();
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
            SHA256 mySHA256 = SHA256.Create();
            return mySHA256.ComputeHash(content);
        }

        public static Asn1Set CreateBerSetFromList(IList berObjects)
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
