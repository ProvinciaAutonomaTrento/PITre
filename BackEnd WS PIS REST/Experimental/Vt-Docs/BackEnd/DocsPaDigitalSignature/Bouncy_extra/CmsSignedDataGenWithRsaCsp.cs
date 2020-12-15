using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1;
using Asn1 = Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.X509;

using Org.BouncyCastle.Cms;
using NetCrypto = System.Security.Cryptography;

namespace CryptoUpgNet.NonExportablePK
{
    public class CmsSignedDataGenWithRsaCsp
        : CmsSignedGenerator
    {
        private static readonly CmsSignedHelper Helper = CmsSignedHelper.Instance;

        private readonly ArrayList signerInfs = new ArrayList();

        private class SignerInf
        {
            private readonly CmsSignedGenerator outer;

            private readonly AsymmetricKeyParameter key;
            private readonly NetCrypto.RSACryptoServiceProvider krProv;
            private readonly SignerIdentifier signerIdentifier;
            private readonly string digestOID;
            private readonly string encOID;
            private readonly CmsAttributeTableGenerator sAttr;
            private readonly CmsAttributeTableGenerator unsAttr;
            private readonly Asn1.Cms.AttributeTable baseSignedTable;

            /// <summary>
            /// Signer is received with crypto provider or pr. key (if the crypto provider == null)
            /// </summary>
            /// <param name="krProv">Crypto provider. if null than use key</param>
            /// <param name="key">Private key</param>
            internal SignerInf(
                CmsSignedGenerator outer,
                NetCrypto.RSACryptoServiceProvider krProv,
                AsymmetricKeyParameter key,
                SignerIdentifier signerIdentifier,
                string digestOID,
                string encOID,
                CmsAttributeTableGenerator sAttr,
                CmsAttributeTableGenerator unsAttr,
                Asn1.Cms.AttributeTable baseSignedTable)
            {
                this.outer = outer;
                this.key = key;
                this.krProv = krProv;
                this.signerIdentifier = signerIdentifier;
                this.digestOID = digestOID;
                this.encOID = encOID;
                this.sAttr = sAttr;
                this.unsAttr = unsAttr;
                this.baseSignedTable = baseSignedTable;
            }

            internal AlgorithmIdentifier DigestAlgorithmID
            {
                get { return new AlgorithmIdentifier(new DerObjectIdentifier(digestOID), DerNull.Instance); }
            }

            internal CmsAttributeTableGenerator SignedAttributes
            {
                get { return sAttr; }
            }

            internal CmsAttributeTableGenerator UnsignedAttributes
            {
                get { return unsAttr; }
            }

            internal SignerInfo ToSignerInfo(
                DerObjectIdentifier contentType,
                CmsProcessable content,
                SecureRandom random,
                bool isCounterSignature)
            {
                AlgorithmIdentifier digAlgId = DigestAlgorithmID;
                string digestName = Helper.GetDigestAlgName(digestOID);

                IDigest dig = Helper.GetDigestInstance(digestName);

                string signatureName = digestName + "with" + Helper.GetEncryptionAlgName(encOID);
                //ISigner sig moved there where used

                // TODO Optimise the case where more than one signer with same digest
                if (content != null)
                {
                    content.Write(new DigOutputStream(dig));
                }

                byte[] hash = DigestUtilities.DoFinal(dig);
                outer._digests.Add(digestOID, hash.Clone());

                Asn1Set signedAttr = null;
                byte[] tmp;
                if (sAttr != null)
                {
                    IDictionary parameters = outer.GetBaseParameters(contentType, digAlgId, hash);

                    //					Asn1.Cms.AttributeTable signed = sAttr.GetAttributes(Collections.unmodifiableMap(parameters));
                    Asn1.Cms.AttributeTable signed = sAttr.GetAttributes(parameters);

                    if (isCounterSignature)
                    {
                        Hashtable tmpSigned = signed.ToHashtable();
                        tmpSigned.Remove(CmsAttributes.ContentType);
                        signed = new Asn1.Cms.AttributeTable(tmpSigned);
                    }

                    // TODO Validate proposed signed attributes

                    signedAttr = outer.GetAttributeSet(signed);

                    // sig must be composed from the DER encoding.
                    tmp = signedAttr.GetEncoded(Asn1Encodable.Der);
                }
                else
                {
                    // TODO Use raw signature of the hash value instead
                    MemoryStream bOut = new MemoryStream();
                    if (content != null)
                    {
                        content.Write(bOut);
                    }
                    tmp = bOut.ToArray();
                }

                byte[] sigBytes = null;
                if (krProv != null)
                {
                    /*
                    sig.GenerateSignature() supports the following hashes:
                    MD2
                    MD4
                    MD5
                    SHA1
                    SHA224
                    SHA256
                    SHA384
                    SHA512
                    RIPEMD128
                    RIPEMD160
                    RIPEMD256
                    *
                    krProv.SignData(tmp, digestName) supports the following digestName:
                    MD5
                    SHA1
                    SHA256
                    SHA384
                    SHA512
                    */

                    //sigBytes = krProv.SignData(tmp, digestName);

                    IDigest digProv = Helper.GetDigestInstance(digestName);
                    digProv.BlockUpdate(tmp, 0, tmp.Length);
                    byte[] hashProv = new byte[digProv.GetDigestSize()];
                    digProv.DoFinal(hashProv, 0);

                    sigBytes = krProv.SignHash(hashProv, digestOID);

                }
                else
                {
                    ISigner sig = Helper.GetSignatureInstance(signatureName);//was moved
                    sig.Init(true, new ParametersWithRandom(key, random));
                    sig.BlockUpdate(tmp, 0, tmp.Length);
                    sigBytes = sig.GenerateSignature();
                }

                Asn1Set unsignedAttr = null;
                if (unsAttr != null)
                {
                    IDictionary baseParameters = outer.GetBaseParameters(contentType, digAlgId, hash);
                    baseParameters[CmsAttributeTableParameter.Signature] = sigBytes.Clone();

                    //					Asn1.Cms.AttributeTable unsigned = unsAttr.GetAttributes(Collections.unmodifiableMap(baseParameters));
                    Asn1.Cms.AttributeTable unsigned = unsAttr.GetAttributes(baseParameters);

                    // TODO Validate proposed unsigned attributes

                    unsignedAttr = outer.GetAttributeSet(unsigned);
                }

                // TODO [RSAPSS] Need the ability to specify non-default parameters
                Asn1Encodable sigX509Parameters = SignerUtilities.GetDefaultX509Parameters(signatureName);
                AlgorithmIdentifier encAlgId = CmsSignedGenerator.GetEncAlgorithmIdentifier(
                    new DerObjectIdentifier(encOID), sigX509Parameters);

                return new SignerInfo(signerIdentifier, digAlgId,
                    signedAttr, encAlgId, new DerOctetString(sigBytes), unsignedAttr);
            }
        }

        public CmsSignedDataGenWithRsaCsp()
        {
        }

        /// <summary>Constructor allowing specific source of randomness</summary>
        /// <param name="rand">Instance of <c>SecureRandom</c> to use.</param>
        public CmsSignedDataGenWithRsaCsp(
            SecureRandom rand)
            : base(rand)
        {
        }

        /// <summary>
        /// add a signer - no attributes other than the default ones will be provided here ??? RSACryptoServiceProvider
        /// </summary>
        /// <param name="crProv">RSACryptoServiceProvider ???????????????? ??? ????????????</param>
        /// <param name="cert">cert certificate containing corresponding public key</param>
        /// <param name="digestOID">digestOID digest algorithm OID</param>
        public void AddSigner(
            NetCrypto.RSACryptoServiceProvider crProv,
            X509Certificate cert,
            string digestOID)
        {
            AddSigner(crProv, cert, GetEncOid(crProv, digestOID), digestOID);
        }


        public void MyAddSigner(
        NetCrypto.RSACryptoServiceProvider crProv,
        X509Certificate cert,
        string encryptionOID,
        string digestOID,
        Asn1.Cms.AttributeTable signedAttr,
        Asn1.Cms.AttributeTable unsignedAttr)
        {
            signerInfs.Add(new SignerInf(this, crProv, null, GetSignerIdentifier(cert), digestOID, encryptionOID,
    new DefaultSignedAttributeTableGenerator(signedAttr), new SimpleAttributeTableGenerator(unsignedAttr), null));

        }
        /// <summary>
        /// add a signer, specifying the digest encryption algorithm to use - no attributes other than the default ones will be provided here.
        /// </summary>
        /// <param name="crProv">RSACryptoServiceProvider ???????????????? ??? ????????????</param>
        /// <param name="cert">certificate containing corresponding public key</param>
        /// <param name="encryptionOID">digest encryption algorithm OID</param>
        /// <param name="digestOID">digest algorithm OID</param>
        public void AddSigner(
            NetCrypto.RSACryptoServiceProvider crProv,
            X509Certificate cert,
            string encryptionOID,
            string digestOID)
        {
            signerInfs.Add(new SignerInf(this, crProv, null, GetSignerIdentifier(cert), digestOID, encryptionOID,
                new DefaultSignedAttributeTableGenerator(), null, null));
        }

        /// <summary>
        /// add a signer, including digest encryption algorithm, with extra signed/unsigned attributes based on generators
        /// </summary>
        public void AddSigner(
            NetCrypto.RSACryptoServiceProvider crProv,
            byte[] subjectKeyID,
            string encryptionOID,
            string digestOID,
            CmsAttributeTableGenerator signedAttrGen,
            CmsAttributeTableGenerator unsignedAttrGen)
        {
            signerInfs.Add(new SignerInf(this, crProv, null, GetSignerIdentifier(subjectKeyID),
                digestOID, encryptionOID, signedAttrGen, unsignedAttrGen, null));
        }

        #region AddSigner for AsymmetricKeyParameter

        /**
        * add a signer - no attributes other than the default ones will be
        * provided here.
		*
		* @param key signing key to use
		* @param cert certificate containing corresponding public key
		* @param digestOID digest algorithm OID
        */
        public void AddSigner(
            AsymmetricKeyParameter privateKey,
            X509Certificate cert,
            string digestOID)
        {
            AddSigner(privateKey, cert, GetEncOid(privateKey, digestOID), digestOID);
        }

        /**
         * add a signer, specifying the digest encryption algorithm to use - no attributes other than the default ones will be
         * provided here.
         *
         * @param key signing key to use
         * @param cert certificate containing corresponding public key
         * @param encryptionOID digest encryption algorithm OID
         * @param digestOID digest algorithm OID
         */
        public void AddSigner(
            AsymmetricKeyParameter privateKey,
            X509Certificate cert,
            string encryptionOID,
            string digestOID)
        {
            signerInfs.Add(new SignerInf(this, null, privateKey, GetSignerIdentifier(cert), digestOID, encryptionOID,
                new DefaultSignedAttributeTableGenerator(), null, null));
        }

        /**
         * add a signer - no attributes other than the default ones will be
         * provided here.
         */
        public void AddSigner(
            AsymmetricKeyParameter privateKey,
            byte[] subjectKeyID,
            string digestOID)
        {
            AddSigner(privateKey, subjectKeyID, GetEncOid(privateKey, digestOID), digestOID);
        }

        /**
         * add a signer, specifying the digest encryption algorithm to use - no attributes other than the default ones will be
         * provided here.
         */
        public void AddSigner(
            AsymmetricKeyParameter privateKey,
            byte[] subjectKeyID,
            string encryptionOID,
            string digestOID)
        {
            signerInfs.Add(new SignerInf(this, null, privateKey, GetSignerIdentifier(subjectKeyID),
                digestOID, encryptionOID,
                new DefaultSignedAttributeTableGenerator(), null, null));
        }

        /**
        * add a signer with extra signed/unsigned attributes.
		*
		* @param key signing key to use
		* @param cert certificate containing corresponding public key
		* @param digestOID digest algorithm OID
		* @param signedAttr table of attributes to be included in signature
		* @param unsignedAttr table of attributes to be included as unsigned
        */
        public void AddSigner(
            AsymmetricKeyParameter privateKey,
            X509Certificate cert,
            string digestOID,
            Asn1.Cms.AttributeTable signedAttr,
            Asn1.Cms.AttributeTable unsignedAttr)
        {
            AddSigner(privateKey, cert, GetEncOid(privateKey, digestOID), digestOID,
                signedAttr, unsignedAttr);
        }

        /**
         * add a signer, specifying the digest encryption algorithm, with extra signed/unsigned attributes.
         *
         * @param key signing key to use
         * @param cert certificate containing corresponding public key
         * @param encryptionOID digest encryption algorithm OID
         * @param digestOID digest algorithm OID
         * @param signedAttr table of attributes to be included in signature
         * @param unsignedAttr table of attributes to be included as unsigned
         */
        public void AddSigner(
            AsymmetricKeyParameter privateKey,
            X509Certificate cert,
            string encryptionOID,
            string digestOID,
            Asn1.Cms.AttributeTable signedAttr,
            Asn1.Cms.AttributeTable unsignedAttr)
        {
            signerInfs.Add(new SignerInf(this, null, privateKey, GetSignerIdentifier(cert),
                digestOID, encryptionOID,
                new DefaultSignedAttributeTableGenerator(signedAttr),
                new SimpleAttributeTableGenerator(unsignedAttr),
                signedAttr));
        }

        /**
         * add a signer with extra signed/unsigned attributes.
         *
         * @param key signing key to use
         * @param subjectKeyID subjectKeyID of corresponding public key
         * @param digestOID digest algorithm OID
         * @param signedAttr table of attributes to be included in signature
         * @param unsignedAttr table of attributes to be included as unsigned
         */
        public void AddSigner(
            AsymmetricKeyParameter privateKey,
            byte[] subjectKeyID,
            string digestOID,
            Asn1.Cms.AttributeTable signedAttr,
            Asn1.Cms.AttributeTable unsignedAttr)
        {
            AddSigner(privateKey, subjectKeyID, digestOID, GetEncOid(privateKey, digestOID),
                new DefaultSignedAttributeTableGenerator(signedAttr),
                new SimpleAttributeTableGenerator(unsignedAttr));
        }

        /**
         * add a signer, specifying the digest encryption algorithm, with extra signed/unsigned attributes.
         *
         * @param key signing key to use
         * @param subjectKeyID subjectKeyID of corresponding public key
         * @param encryptionOID digest encryption algorithm OID
         * @param digestOID digest algorithm OID
         * @param signedAttr table of attributes to be included in signature
         * @param unsignedAttr table of attributes to be included as unsigned
         */
        public void AddSigner(
            AsymmetricKeyParameter privateKey,
            byte[] subjectKeyID,
            string encryptionOID,
            string digestOID,
            Asn1.Cms.AttributeTable signedAttr,
            Asn1.Cms.AttributeTable unsignedAttr)
        {
            signerInfs.Add(new SignerInf(this, null, privateKey, GetSignerIdentifier(subjectKeyID),
                digestOID, encryptionOID,
                new DefaultSignedAttributeTableGenerator(signedAttr),
                new SimpleAttributeTableGenerator(unsignedAttr),
                signedAttr));
        }

        /**
         * add a signer with extra signed/unsigned attributes based on generators.
         */
        public void AddSigner(
            AsymmetricKeyParameter privateKey,
            X509Certificate cert,
            string digestOID,
            CmsAttributeTableGenerator signedAttrGen,
            CmsAttributeTableGenerator unsignedAttrGen)
        {
            AddSigner(privateKey, cert, GetEncOid(privateKey, digestOID), digestOID,
                signedAttrGen, unsignedAttrGen);
        }

        /**
         * add a signer, specifying the digest encryption algorithm, with extra signed/unsigned attributes based on generators.
         */
        public void AddSigner(
            AsymmetricKeyParameter privateKey,
            X509Certificate cert,
            string encryptionOID,
            string digestOID,
            CmsAttributeTableGenerator signedAttrGen,
            CmsAttributeTableGenerator unsignedAttrGen)
        {
            signerInfs.Add(new SignerInf(this, null, privateKey, GetSignerIdentifier(cert),
                digestOID, encryptionOID,
                signedAttrGen, unsignedAttrGen, null));
        }

        /**
         * add a signer with extra signed/unsigned attributes based on generators.
         */
        public void AddSigner(
            AsymmetricKeyParameter privateKey,
            byte[] subjectKeyID,
            string digestOID,
            CmsAttributeTableGenerator signedAttrGen,
            CmsAttributeTableGenerator unsignedAttrGen)
        {
            AddSigner(privateKey, subjectKeyID, digestOID, GetEncOid(privateKey, digestOID),
                signedAttrGen, unsignedAttrGen);
        }

        /**
         * add a signer, including digest encryption algorithm, with extra signed/unsigned attributes based on generators.
         */
        public void AddSigner(
            AsymmetricKeyParameter privateKey,
            byte[] subjectKeyID,
            string encryptionOID,
            string digestOID,
            CmsAttributeTableGenerator signedAttrGen,
            CmsAttributeTableGenerator unsignedAttrGen)
        {
            signerInfs.Add(new SignerInf(this, null, privateKey, GetSignerIdentifier(subjectKeyID),
                digestOID, encryptionOID, signedAttrGen, unsignedAttrGen, null));
        }

        #endregion

        protected string GetEncOid(
            NetCrypto.RSACryptoServiceProvider crProv,
            string digestOID)
        {
            string encOID = null;

            if (crProv is NetCrypto.RSACryptoServiceProvider)
            {
                if ((crProv).PublicOnly)
                    throw new ArgumentException("Expected RSA private key");

                encOID = EncryptionRsa;
            }
            /*else if (key is DsaPrivateKeyParameters)
            {
                if (!digestOID.Equals(DigestSha1))
                    throw new ArgumentException("can't mix DSA with anything but SHA1");

                encOID = EncryptionDsa;
            }
            else if (key is ECPrivateKeyParameters)
            {
                ECPrivateKeyParameters ecPrivKey = (ECPrivateKeyParameters)key;
                string algName = ecPrivKey.AlgorithmName;

                if (algName == "ECGOST3410")
                {
                    encOID = EncryptionECGost3410;
                }
                else
                {
                    // TO DO Should we insist on algName being one of "EC" or "ECDSA", as Java does?
                    encOID = (string)ecAlgorithms[digestOID];

                    if (encOID == null)
                        throw new ArgumentException("can't mix ECDSA with anything but SHA family digests");
                }
            }
            else if (key is Gost3410PrivateKeyParameters)
            {
                encOID = EncryptionGost3410;
            }
            else
            {
                throw new ArgumentException("Unknown algorithm in CmsSignedGenerator.GetEncOid");
            }*/

            return encOID;
        }


        /**
        * generate a signed object that for a CMS Signed Data object
        */
        public CmsSignedData Generate(
            CmsProcessable content)
        {
            return Generate(content, false);
        }

        /**
        * generate a signed object that for a CMS Signed Data
        * object  - if encapsulate is true a copy
        * of the message will be included in the signature. The content type
        * is set according to the OID represented by the string signedContentType.
        */
        public CmsSignedData Generate(
            string signedContentType,
            CmsProcessable content,
            bool encapsulate)
        {
            Asn1EncodableVector digestAlgs = new Asn1EncodableVector();
            Asn1EncodableVector signerInfos = new Asn1EncodableVector();

            _digests.Clear(); // clear the current preserved digest state

            //
            // add the precalculated SignerInfo objects.
            //
            foreach (SignerInformation signer in _signers)
            {
                digestAlgs.Add(Helper.FixAlgID(signer.DigestAlgorithmID));
                signerInfos.Add(signer.ToSignerInfo());
            }

            //
            // add the SignerInfo objects
            //
            bool isCounterSignature = (signedContentType == null);

            DerObjectIdentifier contentTypeOID = isCounterSignature
                ? CmsObjectIdentifiers.Data
                : new DerObjectIdentifier(signedContentType);

            foreach (SignerInf signer in signerInfs)
            {
                try
                {
                    digestAlgs.Add(signer.DigestAlgorithmID);
                    signerInfos.Add(signer.ToSignerInfo(contentTypeOID, content, rand, isCounterSignature));
                }
                catch (IOException e)
                {
                    throw new CmsException("encoding error.", e);
                }
                catch (InvalidKeyException e)
                {
                    throw new CmsException("key inappropriate for signature.", e);
                }
                catch (SignatureException e)
                {
                    throw new CmsException("error creating signature.", e);
                }
                catch (CertificateEncodingException e)
                {
                    throw new CmsException("error creating sid.", e);
                }
            }

            Asn1Set certificates = null;

            if (_certs.Count != 0)
            {
                certificates = CmsUtilities.CreateBerSetFromList(_certs);
            }

            Asn1Set certrevlist = null;

            if (_crls.Count != 0)
            {
                certrevlist = CmsUtilities.CreateBerSetFromList(_crls);
            }

            Asn1OctetString octs = null;
            if (encapsulate)
            {
                MemoryStream bOut = new MemoryStream();
                if (content != null)
                {
                    try
                    {
                        content.Write(bOut);
                    }
                    catch (IOException e)
                    {
                        throw new CmsException("encapsulation error.", e);
                    }
                }
                octs = new BerOctetString(bOut.ToArray());
            }

            ContentInfo encInfo = new ContentInfo(contentTypeOID, octs);

            SignedData sd = new SignedData(
                new DerSet(digestAlgs),
                encInfo,
                certificates,
                certrevlist,
                new DerSet(signerInfos));

            ContentInfo contentInfo = new ContentInfo(CmsObjectIdentifiers.SignedData, sd);

            return new CmsSignedData(content, contentInfo);
        }

        /**
        * generate a signed object that for a CMS Signed Data
        * object - if encapsulate is true a copy
        * of the message will be included in the signature with the
        * default content type "data".
        */
        public CmsSignedData Generate(
            CmsProcessable content,
            bool encapsulate)
        {
            return this.Generate(Data, content, encapsulate);
        }

        /**
        * generate a set of one or more SignerInformation objects representing counter signatures on
        * the passed in SignerInformation object.
        *
        * @param signer the signer to be countersigned
        * @param sigProvider the provider to be used for counter signing.
        * @return a store containing the signers.
        */
        public SignerInformationStore GenerateCounterSigners(
            SignerInformation signer)
        {
            return this.Generate(null, new CmsProcessableByteArray(signer.GetSignature()), false).GetSignerInfos();
        }

        public static bool arrAreEquals(byte[] a, byte[] b)
        {
            bool res = true;

            if (a.Length != b.Length) return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }

            return res;
        }

    }
}