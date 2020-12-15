package com.nttdata.signapplet.gui;

import java.util.Collection;
import java.util.Dictionary;
import java.util.Enumeration;
import java.util.Hashtable;
import java.util.Locale;

import org.bouncycastle.asn1.ASN1Encodable;
import org.bouncycastle.asn1.ASN1Integer;
import org.bouncycastle.asn1.DERInteger;
import org.bouncycastle.asn1.DERNull;
import org.bouncycastle.asn1.DERObjectIdentifier;
import org.bouncycastle.asn1.cryptopro.CryptoProObjectIdentifiers;
import org.bouncycastle.asn1.nist.NISTObjectIdentifiers;
import org.bouncycastle.asn1.pkcs.PKCSObjectIdentifiers;
import org.bouncycastle.asn1.pkcs.RSASSAPSSparams;
import org.bouncycastle.asn1.teletrust.TeleTrusTObjectIdentifiers;
import org.bouncycastle.asn1.x509.AlgorithmIdentifier;
import org.bouncycastle.asn1.x9.X9ObjectIdentifiers;
import org.bouncycastle.crypto.Signer;
import org.bouncycastle.crypto.digests.GOST3411Digest;
import org.bouncycastle.crypto.digests.MD2Digest;
import org.bouncycastle.crypto.digests.MD4Digest;
import org.bouncycastle.crypto.digests.MD5Digest;
import org.bouncycastle.crypto.digests.NullDigest;
import org.bouncycastle.crypto.digests.RIPEMD128Digest;
import org.bouncycastle.crypto.digests.RIPEMD160Digest;
import org.bouncycastle.crypto.digests.SHA1Digest;
import org.bouncycastle.crypto.digests.SHA224Digest;
import org.bouncycastle.crypto.digests.SHA256Digest;
import org.bouncycastle.crypto.digests.SHA384Digest;
import org.bouncycastle.crypto.digests.SHA512Digest;
import org.bouncycastle.crypto.engines.RSABlindedEngine;
import org.bouncycastle.crypto.signers.DSADigestSigner;
import org.bouncycastle.crypto.signers.DSASigner;
import org.bouncycastle.crypto.signers.ECDSASigner;
import org.bouncycastle.crypto.signers.ECGOST3410Signer;
import org.bouncycastle.crypto.signers.ECNRSigner;
import org.bouncycastle.crypto.signers.GOST3410Signer;
import org.bouncycastle.crypto.signers.ISO9796d2Signer;
import org.bouncycastle.crypto.signers.PSSSigner;
import org.bouncycastle.crypto.signers.RSADigestSigner;
import org.bouncycastle.security.DigestUtilities;

public class SignerUtilities {

	static Dictionary algorithms = new Hashtable();
    static Dictionary oids = new Hashtable();

	static 
    {
		algorithms.put("MD2WITHRSA", "MD2withRSA");
        algorithms.put("MD2WITHRSAENCRYPTION", "MD2withRSA");
        algorithms.put(PKCSObjectIdentifiers.md2WithRSAEncryption.getId(), "MD2withRSA");

        algorithms.put("MD4WITHRSA", "MD4withRSA");
        algorithms.put("MD4WITHRSAENCRYPTION", "MD4withRSA");
        algorithms.put(PKCSObjectIdentifiers.md4WithRSAEncryption.getId(), "MD4withRSA");

        algorithms.put("MD5WITHRSA", "MD5withRSA");
        algorithms.put("MD5WITHRSAENCRYPTION", "MD5withRSA");
        algorithms.put(PKCSObjectIdentifiers.md5WithRSAEncryption.getId(), "MD5withRSA");

        algorithms.put("SHA1WITHRSA", "SHA-1withRSA");
        algorithms.put("SHA1WITHRSAENCRYPTION", "SHA-1withRSA");
        algorithms.put(PKCSObjectIdentifiers.sha1WithRSAEncryption.getId(), "SHA-1withRSA");
        algorithms.put("SHA-1WITHRSA", "SHA-1withRSA");

        algorithms.put("SHA224WITHRSA", "SHA-224withRSA");
        algorithms.put("SHA224WITHRSAENCRYPTION", "SHA-224withRSA");
        algorithms.put(PKCSObjectIdentifiers.sha224WithRSAEncryption.getId(), "SHA-224withRSA");
        algorithms.put("SHA-224WITHRSA", "SHA-224withRSA");

        algorithms.put("SHA256WITHRSA", "SHA-256withRSA");
        algorithms.put("SHA256WITHRSAENCRYPTION", "SHA-256withRSA");
        algorithms.put(PKCSObjectIdentifiers.sha256WithRSAEncryption.getId(), "SHA-256withRSA");
        algorithms.put("SHA-256WITHRSA", "SHA-256withRSA");

        algorithms.put("SHA384WITHRSA", "SHA-384withRSA");
        algorithms.put("SHA384WITHRSAENCRYPTION", "SHA-384withRSA");
        algorithms.put(PKCSObjectIdentifiers.sha384WithRSAEncryption.getId(), "SHA-384withRSA");
        algorithms.put("SHA-384WITHRSA", "SHA-384withRSA");

        algorithms.put("SHA512WITHRSA", "SHA-512withRSA");
        algorithms.put("SHA512WITHRSAENCRYPTION", "SHA-512withRSA");
        algorithms.put(PKCSObjectIdentifiers.sha512WithRSAEncryption.getId(), "SHA-512withRSA");
        algorithms.put("SHA-512WITHRSA", "SHA-512withRSA");

		algorithms.put("PSSWITHRSA", "PSSwithRSA");
		algorithms.put("RSASSA-PSS", "PSSwithRSA");
		algorithms.put(PKCSObjectIdentifiers.id_RSASSA_PSS.getId(), "PSSwithRSA");
		algorithms.put("RSAPSS", "PSSwithRSA");

		algorithms.put("SHA1WITHRSAANDMGF1", "SHA-256withRSAandMGF1");
		algorithms.put("SHA-1WITHRSAANDMGF1", "SHA-1withRSAandMGF1");
		algorithms.put("SHA1WITHRSA/PSS", "SHA-1withRSAandMGF1");
		algorithms.put("SHA-1WITHRSA/PSS", "SHA-1withRSAandMGF1");

		algorithms.put("SHA224WITHRSAANDMGF1", "SHA-224withRSAandMGF1");
		algorithms.put("SHA-224WITHRSAANDMGF1", "SHA-224withRSAandMGF1");
		algorithms.put("SHA224WITHRSA/PSS", "SHA-224withRSAandMGF1");
		algorithms.put("SHA-224WITHRSA/PSS", "SHA-224withRSAandMGF1");

		algorithms.put("SHA256WITHRSAANDMGF1", "SHA-256withRSAandMGF1");
        algorithms.put("SHA-256WITHRSAANDMGF1", "SHA-256withRSAandMGF1");
		algorithms.put("SHA256WITHRSA/PSS", "SHA-256withRSAandMGF1");
		algorithms.put("SHA-256WITHRSA/PSS", "SHA-256withRSAandMGF1");

        algorithms.put("SHA384WITHRSAANDMGF1", "SHA-384withRSAandMGF1");
        algorithms.put("SHA-384WITHRSAANDMGF1", "SHA-384withRSAandMGF1");
		algorithms.put("SHA384WITHRSA/PSS", "SHA-384withRSAandMGF1");
		algorithms.put("SHA-384WITHRSA/PSS", "SHA-384withRSAandMGF1");

        algorithms.put("SHA512WITHRSAANDMGF1", "SHA-512withRSAandMGF1");
        algorithms.put("SHA-512WITHRSAANDMGF1", "SHA-512withRSAandMGF1");
		algorithms.put("SHA512WITHRSA/PSS", "SHA-512withRSAandMGF1");
		algorithms.put("SHA-512WITHRSA/PSS", "SHA-512withRSAandMGF1");

		algorithms.put("RIPEMD128WITHRSA", "RIPEMD128withRSA");
        algorithms.put("RIPEMD128WITHRSAENCRYPTION", "RIPEMD128withRSA");
        algorithms.put(TeleTrusTObjectIdentifiers.rsaSignatureWithripemd128.getId(), "RIPEMD128withRSA");

		algorithms.put("RIPEMD160WITHRSA", "RIPEMD160withRSA");
        algorithms.put("RIPEMD160WITHRSAENCRYPTION", "RIPEMD160withRSA");
        algorithms.put(TeleTrusTObjectIdentifiers.rsaSignatureWithripemd160.getId(), "RIPEMD160withRSA");

        algorithms.put("RIPEMD256WITHRSA", "RIPEMD256withRSA");
        algorithms.put("RIPEMD256WITHRSAENCRYPTION", "RIPEMD256withRSA");
        algorithms.put(TeleTrusTObjectIdentifiers.rsaSignatureWithripemd256.getId(), "RIPEMD256withRSA");

		algorithms.put("NONEWITHRSA", "RSA");
		algorithms.put("RSAWITHNONE", "RSA");
		algorithms.put("RAWRSA", "RSA");

		algorithms.put("RAWRSAPSS", "RAWRSASSA-PSS");
		algorithms.put("NONEWITHRSAPSS", "RAWRSASSA-PSS");
		algorithms.put("NONEWITHRSASSA-PSS", "RAWRSASSA-PSS");

		algorithms.put("NONEWITHDSA", "NONEwithDSA");								
		algorithms.put("DSAWITHNONE", "NONEwithDSA");
		algorithms.put("RAWDSA", "NONEwithDSA");

		algorithms.put("DSA", "SHA-1withDSA");
		algorithms.put("DSAWITHSHA1", "SHA-1withDSA");
		algorithms.put("DSAWITHSHA-1", "SHA-1withDSA");
		algorithms.put("SHA/DSA", "SHA-1withDSA");
		algorithms.put("SHA1/DSA", "SHA-1withDSA");
		algorithms.put("SHA-1/DSA", "SHA-1withDSA");
		algorithms.put("SHA1WITHDSA", "SHA-1withDSA");
        algorithms.put("SHA-1WITHDSA", "SHA-1withDSA");
        algorithms.put(X9ObjectIdentifiers.id_dsa_with_sha1.getId(), "SHA-1withDSA");

		algorithms.put("DSAWITHSHA224", "SHA-224withDSA");
		algorithms.put("DSAWITHSHA-224", "SHA-224withDSA");
		algorithms.put("SHA224/DSA", "SHA-224withDSA");
		algorithms.put("SHA-224/DSA", "SHA-224withDSA");
		algorithms.put("SHA224WITHDSA", "SHA-224withDSA");
		algorithms.put("SHA-224WITHDSA", "SHA-224withDSA");
		algorithms.put(NISTObjectIdentifiers.dsa_with_sha224.getId(), "SHA-224withDSA");

		algorithms.put("DSAWITHSHA256", "SHA-256withDSA");
		algorithms.put("DSAWITHSHA-256", "SHA-256withDSA");
		algorithms.put("SHA256/DSA", "SHA-256withDSA");
		algorithms.put("SHA-256/DSA", "SHA-256withDSA");
		algorithms.put("SHA256WITHDSA", "SHA-256withDSA");
		algorithms.put("SHA-256WITHDSA", "SHA-256withDSA");
		algorithms.put(NISTObjectIdentifiers.dsa_with_sha256.getId(), "SHA-256withDSA");

		algorithms.put("DSAWITHSHA384", "SHA-384withDSA");
		algorithms.put("DSAWITHSHA-384", "SHA-384withDSA");
		algorithms.put("SHA384/DSA", "SHA-384withDSA");
		algorithms.put("SHA-384/DSA", "SHA-384withDSA");
		algorithms.put("SHA384WITHDSA", "SHA-384withDSA");
		algorithms.put("SHA-384WITHDSA", "SHA-384withDSA");
		algorithms.put(NISTObjectIdentifiers.dsa_with_sha384.getId(), "SHA-384withDSA");

		algorithms.put("DSAWITHSHA512", "SHA-512withDSA");
		algorithms.put("DSAWITHSHA-512", "SHA-512withDSA");
		algorithms.put("SHA512/DSA", "SHA-512withDSA");
		algorithms.put("SHA-512/DSA", "SHA-512withDSA");
		algorithms.put("SHA512WITHDSA", "SHA-512withDSA");
		algorithms.put("SHA-512WITHDSA", "SHA-512withDSA");
		algorithms.put(NISTObjectIdentifiers.dsa_with_sha512.getId(), "SHA-512withDSA");

		algorithms.put("NONEWITHECDSA", "NONEwithECDSA");
		algorithms.put("ECDSAWITHNONE", "NONEwithECDSA");

		algorithms.put("ECDSA", "SHA-1withECDSA");
		algorithms.put("SHA1/ECDSA", "SHA-1withECDSA");
		algorithms.put("SHA-1/ECDSA", "SHA-1withECDSA");
		algorithms.put("ECDSAWITHSHA1", "SHA-1withECDSA");
		algorithms.put("ECDSAWITHSHA-1", "SHA-1withECDSA");
		algorithms.put("SHA1WITHECDSA", "SHA-1withECDSA");
        algorithms.put("SHA-1WITHECDSA", "SHA-1withECDSA");
		algorithms.put(X9ObjectIdentifiers.ecdsa_with_SHA1.getId(), "SHA-1withECDSA");
		algorithms.put(TeleTrusTObjectIdentifiers.ecSignWithSha1.getId(), "SHA-1withECDSA");

		algorithms.put("SHA224/ECDSA", "SHA-224withECDSA");
		algorithms.put("SHA-224/ECDSA", "SHA-224withECDSA");
		algorithms.put("ECDSAWITHSHA224", "SHA-224withECDSA");
		algorithms.put("ECDSAWITHSHA-224", "SHA-224withECDSA");
		algorithms.put("SHA224WITHECDSA", "SHA-224withECDSA");
		algorithms.put("SHA-224WITHECDSA", "SHA-224withECDSA");
		algorithms.put(X9ObjectIdentifiers.ecdsa_with_SHA224.getId(), "SHA-224withECDSA");

		algorithms.put("SHA256/ECDSA", "SHA-256withECDSA");
		algorithms.put("SHA-256/ECDSA", "SHA-256withECDSA");
		algorithms.put("ECDSAWITHSHA256", "SHA-256withECDSA");
		algorithms.put("ECDSAWITHSHA-256", "SHA-256withECDSA");
		algorithms.put("SHA256WITHECDSA", "SHA-256withECDSA");
		algorithms.put("SHA-256WITHECDSA", "SHA-256withECDSA");
		algorithms.put(X9ObjectIdentifiers.ecdsa_with_SHA256.getId(), "SHA-256withECDSA");

		algorithms.put("SHA384/ECDSA", "SHA-384withECDSA");
		algorithms.put("SHA-384/ECDSA", "SHA-384withECDSA");
		algorithms.put("ECDSAWITHSHA384", "SHA-384withECDSA");
		algorithms.put("ECDSAWITHSHA-384", "SHA-384withECDSA");
		algorithms.put("SHA384WITHECDSA", "SHA-384withECDSA");
		algorithms.put("SHA-384WITHECDSA", "SHA-384withECDSA");
		algorithms.put(X9ObjectIdentifiers.ecdsa_with_SHA384.getId(), "SHA-384withECDSA");

		algorithms.put("SHA512/ECDSA", "SHA-512withECDSA");
		algorithms.put("SHA-512/ECDSA", "SHA-512withECDSA");
		algorithms.put("ECDSAWITHSHA512", "SHA-512withECDSA");
		algorithms.put("ECDSAWITHSHA-512", "SHA-512withECDSA");
		algorithms.put("SHA512WITHECDSA", "SHA-512withECDSA");
		algorithms.put("SHA-512WITHECDSA", "SHA-512withECDSA");
		algorithms.put(X9ObjectIdentifiers.ecdsa_with_SHA512.getId(), "SHA-512withECDSA");

		algorithms.put("RIPEMD160/ECDSA", "RIPEMD160withECDSA");
		algorithms.put("SHA-512/ECDSA", "RIPEMD160withECDSA");
		algorithms.put("ECDSAWITHRIPEMD160", "RIPEMD160withECDSA");
		algorithms.put("ECDSAWITHRIPEMD160", "RIPEMD160withECDSA");
		algorithms.put("RIPEMD160WITHECDSA", "RIPEMD160withECDSA");
		algorithms.put("RIPEMD160WITHECDSA", "RIPEMD160withECDSA");
		algorithms.put(TeleTrusTObjectIdentifiers.rsaSignatureWithripemd160.getId(), "RIPEMD160withECDSA");

		algorithms.put("GOST-3410", "GOST3410");
		algorithms.put("GOST-3410-94", "GOST3410");
		algorithms.put("GOST3411WITHGOST3410", "GOST3410");
		algorithms.put(CryptoProObjectIdentifiers.gostR3411_94_with_gostR3410_94.getId(), "GOST3410");

		algorithms.put("ECGOST-3410", "ECGOST3410");
		algorithms.put("ECGOST-3410-2001", "ECGOST3410");
		algorithms.put("GOST3411WITHECGOST3410", "ECGOST3410");
		algorithms.put(CryptoProObjectIdentifiers.gostR3411_94_with_gostR3410_2001.getId(), "ECGOST3410");



		oids.put("MD2withRSA", PKCSObjectIdentifiers.md2WithRSAEncryption);
        oids.put("MD4withRSA", PKCSObjectIdentifiers.md4WithRSAEncryption);
        oids.put("MD5withRSA", PKCSObjectIdentifiers.md5WithRSAEncryption);

        oids.put("SHA-1withRSA", PKCSObjectIdentifiers.sha1WithRSAEncryption);
        oids.put("SHA-224withRSA", PKCSObjectIdentifiers.sha224WithRSAEncryption);
        oids.put("SHA-256withRSA", PKCSObjectIdentifiers.sha256WithRSAEncryption);
        oids.put("SHA-384withRSA", PKCSObjectIdentifiers.sha384WithRSAEncryption);
        oids.put("SHA-512withRSA", PKCSObjectIdentifiers.sha512WithRSAEncryption);

		oids.put("PSSwithRSA", PKCSObjectIdentifiers.id_RSASSA_PSS);
		oids.put("SHA-1withRSAandMGF1", PKCSObjectIdentifiers.id_RSASSA_PSS);
		oids.put("SHA-224withRSAandMGF1", PKCSObjectIdentifiers.id_RSASSA_PSS);
		oids.put("SHA-256withRSAandMGF1", PKCSObjectIdentifiers.id_RSASSA_PSS);
		oids.put("SHA-384withRSAandMGF1", PKCSObjectIdentifiers.id_RSASSA_PSS);
		oids.put("SHA-512withRSAandMGF1", PKCSObjectIdentifiers.id_RSASSA_PSS);

		oids.put("RIPEMD128withRSA", TeleTrusTObjectIdentifiers.rsaSignatureWithripemd128);
		oids.put("RIPEMD160withRSA", TeleTrusTObjectIdentifiers.rsaSignatureWithripemd160);
		oids.put("RIPEMD256withRSA", TeleTrusTObjectIdentifiers.rsaSignatureWithripemd256);

		oids.put("SHA-1withDSA", X9ObjectIdentifiers.id_dsa_with_sha1);

		oids.put("SHA-1withECDSA", X9ObjectIdentifiers.ecdsa_with_SHA1);
		oids.put("SHA-224withECDSA", X9ObjectIdentifiers.ecdsa_with_SHA224);
		oids.put("SHA-256withECDSA", X9ObjectIdentifiers.ecdsa_with_SHA256);
		oids.put("SHA-384withECDSA", X9ObjectIdentifiers.ecdsa_with_SHA384);
		oids.put("SHA-512withECDSA", X9ObjectIdentifiers.ecdsa_with_SHA512);

		oids.put("GOST3410", CryptoProObjectIdentifiers.gostR3411_94_with_gostR3410_94);
		oids.put("ECGOST3410", CryptoProObjectIdentifiers.gostR3411_94_with_gostR3410_2001);
	}

	/// <summary>
    /// Returns a ObjectIdentifier for a give encoding.
    /// </summary>
    /// <param name="mechanism">A String representation of the encoding.</param>
    /// <returns>A DerObjectIdentifier, null if the Oid is not available.</returns>
	// TODO Don't really want to support this
    public static DERObjectIdentifier GetObjectIdentifier(
		String mechanism)
    {
		if (mechanism == null)
			throw new IllegalArgumentException("mechanism");

		mechanism = mechanism.toUpperCase(Locale.getDefault());
		String aliased = (String) algorithms.get(mechanism);

		if (aliased != null)
			mechanism = aliased;

		return (DERObjectIdentifier) oids.get(mechanism);
	}

	public static Enumeration Algorithms()
    {
        return oids.keys();
    }

	public static ASN1Encodable GetDefaultX509Parameters(
		DERObjectIdentifier id) throws Exception
	{
		return GetDefaultX509Parameters(id.getId());
	}

	public static ASN1Encodable GetDefaultX509Parameters(
		String algorithm) throws Exception
	{
		if (algorithm == null)
			throw new IllegalArgumentException("algorithm");

		algorithm = algorithm.toUpperCase(Locale.getDefault());

		String mechanism = (String) algorithms.get(algorithm);

		if (mechanism == null)
			mechanism = algorithm;

		if (mechanism == "PSSwithRSA")
		{
			// TODO The SHA1Digest here is a default. In JCE version, the actual digest
			// to be used can be overridden by subsequent parameter settings.
			return GetPssX509Parameters("SHA-1");
		}

		if (mechanism.endsWith("withRSAandMGF1"))
		{
			String digestName = mechanism.substring(0, mechanism.length() - "withRSAandMGF1".length());
			return GetPssX509Parameters(digestName);
		}

		return DERNull.INSTANCE;
	}

	private static ASN1Encodable GetPssX509Parameters(
		String	digestName) throws Exception
	{
		AlgorithmIdentifier hashAlgorithm = new AlgorithmIdentifier(
			DigestUtilities.GetObjectIdentifier(digestName), DERNull.INSTANCE);

		// TODO Is it possible for the MGF hash alg to be different from the PSS one?
		AlgorithmIdentifier maskGenAlgorithm = new AlgorithmIdentifier(
			PKCSObjectIdentifiers.id_mgf1, hashAlgorithm);

		int saltLen = DigestUtilities.getDigest(digestName).getDigestSize();
		return new RSASSAPSSparams(hashAlgorithm, maskGenAlgorithm,
			new ASN1Integer(saltLen), new ASN1Integer(1));
	}

	public static Signer GetSigner(
		DERObjectIdentifier id) throws Exception
    {
        return GetSigner(id.getId());
    }

	public static Signer GetSigner(
		String algorithm) throws Exception
    {
		if (algorithm == null)
			throw new IllegalArgumentException("algorithm");

		algorithm = algorithm.toUpperCase(Locale.getDefault());

		String mechanism = (String) algorithms.get(algorithm);

		if (mechanism == null)
			mechanism = algorithm;

		if (mechanism.equals("RSA"))
		{
			return (new RSADigestSigner(new NullDigest()));
		}
		if (mechanism.equals("MD2withRSA"))
        {
            return (new RSADigestSigner(new MD2Digest()));
        }
        if (mechanism.equals("MD4withRSA"))
        {
            return (new RSADigestSigner(new MD4Digest()));
        }
        if (mechanism.equals("MD5withRSA"))
        {
            return (new RSADigestSigner(new MD5Digest()));
        }
        if (mechanism.equals("SHA-1withRSA"))
        {
            return (new RSADigestSigner(new SHA1Digest()));
        }
        if (mechanism.equals("SHA-224withRSA"))
        {
            return (new RSADigestSigner(new SHA224Digest()));
        }
        if (mechanism.equals("SHA-256withRSA"))
        {
            return (new RSADigestSigner(new SHA256Digest()));
        }
        if (mechanism.equals("SHA-384withRSA"))
        {
            return (new RSADigestSigner(new SHA384Digest()));
        }
        if (mechanism.equals("SHA-512withRSA"))
        {
            return (new RSADigestSigner(new SHA512Digest()));
        }
		if (mechanism.equals("RIPEMD128withRSA"))
        {
            return (new RSADigestSigner(new RIPEMD128Digest()));
        }
        if (mechanism.equals("RIPEMD160withRSA"))
        {
            return (new RSADigestSigner(new RIPEMD160Digest()));
        }
        if (mechanism.equals("RIPEMD256withRSA"))
        {
            return (new RSADigestSigner(new RIPEMD128Digest()));
        }

		if (mechanism.equals("RAWRSASSA-PSS"))
		{
			// TODO Add support for other parameter settings
			return PSSSigner.CreateRawSigner(new NullDigest(), new RSABlindedEngine(), new SHA1Digest());
		}
		if (mechanism.equals("PSSwithRSA"))
		{
			// TODO The SHA1Digest here is a default. In JCE version, the actual digest
			// to be used can be overridden by subsequent parameter settings.
			return (new PSSSigner(new RSABlindedEngine(), new SHA1Digest()));
		}
		if (mechanism.equals("SHA-1withRSAandMGF1"))
		{
			return (new PSSSigner(new RSABlindedEngine(), new SHA1Digest()));
		}
		if (mechanism.equals("SHA-224withRSAandMGF1"))
		{
			return (new PSSSigner(new RSABlindedEngine(), new SHA224Digest()));
		}
		if (mechanism.equals("SHA-256withRSAandMGF1"))
		{
			return (new PSSSigner(new RSABlindedEngine(), new SHA256Digest()));
		}
		if (mechanism.equals("SHA-384withRSAandMGF1"))
		{
			return (new PSSSigner(new RSABlindedEngine(), new SHA384Digest()));
		}
		if (mechanism.equals("SHA-512withRSAandMGF1"))
		{
			return (new PSSSigner(new RSABlindedEngine(), new SHA512Digest()));
		}

		if (mechanism.equals("NONEwithDSA"))
		{
			return (new DSADigestSigner(new DSASigner(), new NullDigest()));
		}
		if (mechanism.equals("SHA-1withDSA"))
        {
            return (new DSADigestSigner(new DSASigner(), new SHA1Digest()));
        }
		if (mechanism.equals("SHA-224withDSA"))
		{
			return (new DSADigestSigner(new DSASigner(), new SHA224Digest()));
		}
		if (mechanism.equals("SHA-256withDSA"))
		{
			return (new DSADigestSigner(new DSASigner(), new SHA256Digest()));
		}
		if (mechanism.equals("SHA-384withDSA"))
		{
			return (new DSADigestSigner(new DSASigner(), new SHA384Digest()));
		}
		if (mechanism.equals("SHA-512withDSA"))
		{
			return (new DSADigestSigner(new DSASigner(), new SHA512Digest()));
		}

		if (mechanism.equals("NONEwithECDSA"))
		{
			return (new DSADigestSigner(new ECDSASigner(), new NullDigest()));
		}
		if (mechanism.equals("SHA-1withECDSA"))
        {
            return (new DSADigestSigner(new ECDSASigner(), new SHA1Digest()));
        }
		if (mechanism.equals("SHA-224withECDSA"))
		{
			return (new DSADigestSigner(new ECDSASigner(), new SHA224Digest()));
		}
		if (mechanism.equals("SHA-256withECDSA"))
		{
			return (new DSADigestSigner(new ECDSASigner(), new SHA256Digest()));
		}
		if (mechanism.equals("SHA-384withECDSA"))
		{
			return (new DSADigestSigner(new ECDSASigner(), new SHA384Digest()));
		}
		if (mechanism.equals("SHA-512withECDSA"))
		{
			return (new DSADigestSigner(new ECDSASigner(), new SHA512Digest()));
		}

		if (mechanism.equals("RIPEMD160withECDSA"))
		{
			return (new DSADigestSigner(new ECDSASigner(), new RIPEMD160Digest()));
		}

		if (mechanism.equals("SHA1WITHECNR"))
		{
			return (new DSADigestSigner(new ECNRSigner(), new SHA1Digest()));
		}
		if (mechanism.equals("SHA224WITHECNR"))
		{
			return (new DSADigestSigner(new ECNRSigner(), new SHA224Digest()));
		}
		if (mechanism.equals("SHA256WITHECNR"))
		{
			return (new DSADigestSigner(new ECNRSigner(), new SHA256Digest()));
		}
		if (mechanism.equals("SHA384WITHECNR"))
		{
			return (new DSADigestSigner(new ECNRSigner(), new SHA384Digest()));
		}
		if (mechanism.equals("SHA512WITHECNR"))
		{
			return (new DSADigestSigner(new ECNRSigner(), new SHA512Digest()));
		}

		if (mechanism.equals("GOST3410"))
		{
			throw new Exception("GOST3410DigestSigner not implemented");			
			//return new GOST3410DigestSigner(new GOST3410Signer(), new GOST3411Digest());
		}
		if (mechanism.equals("ECGOST3410"))
		{
			throw new Exception("GOST3410DigestSigner not implemented");
			//return new Gost3410DigestSigner(new ECGOST3410Signer(), new GOST3411Digest());
		}

		if (mechanism.equals("SHA1WITHRSA/ISO9796-2"))
		{
			return new ISO9796d2Signer(new RSABlindedEngine(), new SHA1Digest(), true);
		}
		if (mechanism.equals("MD5WITHRSA/ISO9796-2"))
		{
			return new ISO9796d2Signer(new RSABlindedEngine(), new MD5Digest(), true);
		}
		if (mechanism.equals("RIPEMD160WITHRSA/ISO9796-2"))
		{
			return new ISO9796d2Signer(new RSABlindedEngine(), new RIPEMD160Digest(), true);
		}

		throw new SecurityException("Signer " + algorithm + " not recognised.");
    }

    public static String GetEncodingName(
		DERObjectIdentifier oid)
    {
        return (String) algorithms.get(oid.getId());
    }
}

