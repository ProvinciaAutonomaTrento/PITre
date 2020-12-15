package it.integra.signapplet.gui;

import java.io.ByteArrayInputStream;
import java.util.ArrayList;
import java.util.Dictionary;
import java.util.Hashtable;
import java.util.List;

import org.bouncycastle.asn1.ASN1Choice;
import org.bouncycastle.asn1.ASN1Encodable;
import org.bouncycastle.asn1.ASN1Object;
import org.bouncycastle.asn1.ASN1Sequence;
import org.bouncycastle.asn1.ASN1Set;
import org.bouncycastle.asn1.ASN1TaggedObject;
import org.bouncycastle.asn1.DERObjectIdentifier;
import org.bouncycastle.asn1.cryptopro.CryptoProObjectIdentifiers;
import org.bouncycastle.asn1.eac.EACObjectIdentifiers;
import org.bouncycastle.asn1.nist.NISTObjectIdentifiers;
import org.bouncycastle.asn1.oiw.OIWObjectIdentifiers;
import org.bouncycastle.asn1.pkcs.PKCSObjectIdentifiers;
import org.bouncycastle.asn1.teletrust.TeleTrusTObjectIdentifiers;
import org.bouncycastle.asn1.x509.X509ObjectIdentifiers;
import org.bouncycastle.asn1.x9.X9ObjectIdentifiers;
import org.bouncycastle.cms.CMSException;
import org.bouncycastle.cms.CMSSignedGenerator;
import org.bouncycastle.crypto.Digest;
import org.bouncycastle.crypto.Signer;
import org.bouncycastle.crypto.signers.RSADigestSigner;
import org.bouncycastle.jce.provider.X509CRLParser;
import org.bouncycastle.jce.provider.X509CertParser;
import org.bouncycastle.x509.X509CollectionStoreParameters;
import org.bouncycastle.x509.X509Store;
import org.bouncycastle.x509.X509StoreSpi;
import org.bouncycastle.x509.X509V2AttributeCertificate;


public class CMSSignedHelper_ {
		private static Dictionary<String, String> encryptionAlgs = new Hashtable<String, String>();
        private static Dictionary<String, String> digestAlgs = new Hashtable<String, String>();
        private static Dictionary digestAliases = new Hashtable();
		
        
        
		private static void putEntries(DERObjectIdentifier oid, String digest, String encryption)
		{
			String alias = oid.getId();
			digestAlgs.put(alias, digest);
			encryptionAlgs.put(alias, encryption);
		}

		static
		{
			putEntries(NISTObjectIdentifiers.dsa_with_sha224, "SHA224", "DSA");
			putEntries(NISTObjectIdentifiers.dsa_with_sha256, "SHA256", "DSA");
			putEntries(NISTObjectIdentifiers.dsa_with_sha384, "SHA384", "DSA");
			putEntries(NISTObjectIdentifiers.dsa_with_sha512, "SHA512", "DSA");
			putEntries(OIWObjectIdentifiers.dsaWithSHA1, "SHA1", "DSA");
			putEntries(OIWObjectIdentifiers.md4WithRSA, "MD4", "RSA");
			putEntries(OIWObjectIdentifiers.md4WithRSAEncryption, "MD4", "RSA");
			putEntries(OIWObjectIdentifiers.md5WithRSA, "MD5", "RSA");
			putEntries(OIWObjectIdentifiers.sha1WithRSA, "SHA1", "RSA");
			putEntries(PKCSObjectIdentifiers.md2WithRSAEncryption, "MD2", "RSA");
			putEntries(PKCSObjectIdentifiers.md4WithRSAEncryption, "MD4", "RSA");
			putEntries(PKCSObjectIdentifiers.md5WithRSAEncryption, "MD5", "RSA");
			putEntries(PKCSObjectIdentifiers.sha1WithRSAEncryption, "SHA1", "RSA");
			putEntries(PKCSObjectIdentifiers.sha224WithRSAEncryption, "SHA224", "RSA");
			putEntries(PKCSObjectIdentifiers.sha256WithRSAEncryption, "SHA256", "RSA");
			putEntries(PKCSObjectIdentifiers.sha384WithRSAEncryption, "SHA384", "RSA");
			putEntries(PKCSObjectIdentifiers.sha512WithRSAEncryption, "SHA512", "RSA");
			putEntries(X9ObjectIdentifiers.ecdsa_with_SHA1, "SHA1", "ECDSA");
			putEntries(X9ObjectIdentifiers.ecdsa_with_SHA224, "SHA224", "ECDSA");
			putEntries(X9ObjectIdentifiers.ecdsa_with_SHA256, "SHA256", "ECDSA");
			putEntries(X9ObjectIdentifiers.ecdsa_with_SHA384, "SHA384", "ECDSA");
			putEntries(X9ObjectIdentifiers.ecdsa_with_SHA512, "SHA512", "ECDSA");
			putEntries(X9ObjectIdentifiers.id_dsa_with_sha1, "SHA1", "DSA");
			putEntries(EACObjectIdentifiers.id_TA_ECDSA_SHA_1, "SHA1", "ECDSA");
			putEntries(EACObjectIdentifiers.id_TA_ECDSA_SHA_224, "SHA224", "ECDSA");
			putEntries(EACObjectIdentifiers.id_TA_ECDSA_SHA_256, "SHA256", "ECDSA");
			putEntries(EACObjectIdentifiers.id_TA_ECDSA_SHA_384, "SHA384", "ECDSA");
			putEntries(EACObjectIdentifiers.id_TA_ECDSA_SHA_512, "SHA512", "ECDSA");
			putEntries(EACObjectIdentifiers.id_TA_RSA_v1_5_SHA_1, "SHA1", "RSA");
			putEntries(EACObjectIdentifiers.id_TA_RSA_v1_5_SHA_256, "SHA256", "RSA");
			putEntries(EACObjectIdentifiers.id_TA_RSA_PSS_SHA_1, "SHA1", "RSAandMGF1");
			putEntries(EACObjectIdentifiers.id_TA_RSA_PSS_SHA_256, "SHA256", "RSAandMGF1");

			encryptionAlgs.put(X9ObjectIdentifiers.id_dsa.getId(), "DSA");
			encryptionAlgs.put(PKCSObjectIdentifiers.rsaEncryption.getId(), "RSA");
			encryptionAlgs.put(TeleTrusTObjectIdentifiers.teleTrusTRSAsignatureAlgorithm.getId(), "RSA");
			encryptionAlgs.put(X509ObjectIdentifiers.id_ea_rsa.getId(), "RSA");
			encryptionAlgs.put(CMSSignedGenerator.ENCRYPTION_RSA_PSS, "RSAandMGF1");
			encryptionAlgs.put(CryptoProObjectIdentifiers.gostR3410_94.getId(), "GOST3410");
			encryptionAlgs.put(CryptoProObjectIdentifiers.gostR3410_2001.getId(), "ECGOST3410");
			encryptionAlgs.put("1.3.6.1.4.1.5849.1.6.2", "ECGOST3410");
			encryptionAlgs.put("1.3.6.1.4.1.5849.1.1.5", "GOST3410");

			digestAlgs.put(PKCSObjectIdentifiers.md2.getId(), "MD2");
			digestAlgs.put(PKCSObjectIdentifiers.md4.getId(), "MD4");
			digestAlgs.put(PKCSObjectIdentifiers.md5.getId(), "MD5");
			digestAlgs.put(OIWObjectIdentifiers.idSHA1.getId(), "SHA1");
			digestAlgs.put(NISTObjectIdentifiers.id_sha224.getId(), "SHA224");
			digestAlgs.put(NISTObjectIdentifiers.id_sha256.getId(), "SHA256");
			digestAlgs.put(NISTObjectIdentifiers.id_sha384.getId(), "SHA384");
			digestAlgs.put(NISTObjectIdentifiers.id_sha512.getId(), "SHA512");
			digestAlgs.put(TeleTrusTObjectIdentifiers.ripemd128.getId(), "RIPEMD128");
			digestAlgs.put(TeleTrusTObjectIdentifiers.ripemd160.getId(), "RIPEMD160");
			digestAlgs.put(TeleTrusTObjectIdentifiers.ripemd256.getId(), "RIPEMD256");
			digestAlgs.put(CryptoProObjectIdentifiers.gostR3411.getId(),  "GOST3411");
			digestAlgs.put("1.3.6.1.4.1.5849.1.2.1",  "GOST3411");

			digestAliases.put("SHA1", new String[] { "SHA-1" });
			digestAliases.put("SHA224", new String[] { "SHA-224" });
			digestAliases.put("SHA256", new String[] { "SHA-256" });
			digestAliases.put("SHA384", new String[] { "SHA-384" });
			digestAliases.put("SHA512", new String[] { "SHA-512" });
		}

	        static String getDigestAlgName(
	               String digestAlgOid)
	           {
	   			String algName = (String)digestAlgs.get(digestAlgOid);

	   			if (algName != null)
	   			{
	   				return algName;
	   			}

	   			return digestAlgOid;
	           }

	   		private static String[] getDigestAliases(
	   			String algName)
	   		{
	   			String[] aliases = (String[]) digestAliases.get(algName);

	   			return aliases == null ? new String[0] : (String[]) aliases.clone();
	   		}

	   		/**
	           * Return the digest encryption algorithm using one of the standard
	           * JCA String representations rather than the algorithm identifier (if
	           * possible).
	           */
	           static String GetEncryptionAlgName(
	               String encryptionAlgOid)
	           {
	   			String algName = (String) encryptionAlgs.get(encryptionAlgOid);

	   			if (algName != null)
	   			{
	   				return algName;
	   			}

	   			return encryptionAlgOid;
	           }

	   		static Digest getDigestInstance(
	   			String algorithm) throws Exception
	   		{
	   			try
	   			{
	   				return DigestUtilities.getDigest(algorithm);
	   			}
	   			catch (SecurityException e)
	   			{
	   				// This is probably superfluous on C#, since no provider infrastructure,
	   				// assuming DigestUtilities already knows all the aliases
	   				for (String alias : getDigestAliases(algorithm))
	   				{
	   					try { return DigestUtilities.getDigest(alias); }
	   					catch (SecurityException se) {}
	   				}
	   				throw e;
	   			}
	   		}

	   		private Signer getSignatureInstance(
	   			String algorithm)
	   		{
	   			RSADigestSigner s = new RSADigestSigner (null);
	   			
	   			
	   			return s;
	   			return SignerUtilities.GetSigner(algorithm);
	   		}

	   		private X509Store CreateAttributeStore(
	   			String	type,
	   			ASN1Set	certSet)
	   		{
	   			ArrayList certs = new ArrayList();

	   			if (certSet != null)
	   			{
	   				for (ASN1Encodable ae : certSet.toArray())
	   				{
	   					try
	   					{
	   						ASN1Object obj = ae.toASN1Primitive().toASN1Object();

	   						if (obj instanceof ASN1TaggedObject)
	   						{
	   							ASN1TaggedObject tagged = (ASN1TaggedObject)obj;

	   							if (tagged.getTagNo() == 2)
	   							{
	   								certs.add(
	   									new X509V2AttributeCertificate(
	   										ASN1Sequence.getInstance(tagged, false).getEncoded()));
	   							}
	   						}
	   					}
	   					catch (Exception ex)
	   					{
	   						throw new CMSException("can't re-encode attribute certificate!", ex);
	   					}
	   				}
	   			}

	   			try
	   			{
	   				
	   				
	   				return X509Store.getInstance(
	   					"AttributeCertificate/" + type,
	   					new X509CollectionStoreParameters(certs));
	   			}
	   			catch (Exception e)
	   			{
	   				throw new CMSException("can't setup the X509Store", e);
	   			}
	   		} 

	   		private X509Store CreateCertificateStore(
	   			String	type,
	   			ASN1Set	certSet)
	   		{
	   			ArrayList certs = new ArrayList();

	   			if (certSet != null)
	   			{
	   				AddCertsFromSet(certs, certSet);
	   			}

	   			try
	   			{
	   				return X509StoreFactory.Create(
	   					"Certificate/" + type,
	   					new X509CollectionStoreParameters(certs));
	   			}
	   			catch (ArgumentException e)
	   			{
	   				throw new CmsException("can't setup the X509Store", e);
	   			}
	   		}

	   		private X509Store CreateCrlStore(
	   			String	type,
	   			ASN1Set	crlSet)
	   		{
	   			ArrayList crls = new ArrayList();

	   			if (crlSet != null)
	   			{
	   				addCrlsFromSet(crls, crlSet);
	   			}

	   			try
	   			{
	   				return X509StoreFactory.Create(
	   					"CRL/" + type,
	   					new X509CollectionStoreParameters(crls));
	   			}
	   			catch (ArgumentException e)
	   			{
	   				throw new CmsException("can't setup the X509Store", e);
	   			}
	   		}

	   		private void AddCertsFromSet(
	   			List	certs,
	   			ASN1Set	certSet)
	   		{
	   			X509CertParser cf = new X509CertParser();

	   			for (ASN1Encodable ae : certSet.toArray())
	   			{
	   				try
	   				{
	   					ASN1Object obj = ae.toASN1Primitive().toASN1Object();

	   					if (obj instanceof ASN1Sequence)
	   					{
	   						// TODO Build certificate directly from sequence?
	   						
	   						ByteArrayInputStream bais = new ByteArrayInputStream(obj.getEncoded());
	   						cf.engineInit(bais);
	   						
	   						certs.add(cf.engineRead());
	   						bais.close();	   							   					
	   					}
	   				}
	   				catch (Exception ex)
	   				{
	   					throw new CMSException("can't re-encode certificate!", ex);
	   				}
	   			}
	   		}

	   		private void addCrlsFromSet(
	   			ArrayList	crls,
	   			ASN1Set	crlSet)
	   		{
	   			X509CRLParser cf = new X509CRLParser();
org.bouncycastle.cms.CMSS
	   			while (crlSet.getObjects().hasMoreElements())
	   			{
	   				try
	   				{
	   					ASN1Encodable ae = (ASN1Encodable) crlSet.getObjects().nextElement(); 
	   					// TODO Build CRL directly from ae.ToAsn1Object()?
	   					
	   					
	   					crls.add(cf.readDERCRL(ae.hashCode()));
	   				}
	   				catch (Exception ex)
	   				{
	   					throw new Exception("can't re-encode CRL!", ex);
	   				}
	   			}
	   		}

	   		internal AlgorithmIdentifier FixAlgID(
	   			AlgorithmIdentifier algId)
	   		{
	   			if (algId.Parameters == null)
	   				return new AlgorithmIdentifier(algId.ObjectID, DerNull.Instance);

	   			return algId;
	   		}
	       }
}
