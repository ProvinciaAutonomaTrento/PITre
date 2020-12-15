package com.nttdata.signapplet.gui;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.OutputStream;
import java.security.InvalidKeyException;
import java.security.PrivateKey;
import java.security.SecureRandom;
import java.security.Signature;
import java.security.SignatureException;
import java.security.cert.CertificateEncodingException;
import java.security.cert.X509Certificate;
import java.security.interfaces.RSAPrivateKey;
import java.util.ArrayList;
import java.util.Dictionary;
import java.util.Hashtable;
import java.util.List;
import java.util.Map;

import javax.security.auth.x500.X500Principal;

import org.bouncycastle.asn1.ASN1Encodable;
import org.bouncycastle.asn1.ASN1EncodableVector;
import org.bouncycastle.asn1.ASN1Encoding;
import org.bouncycastle.asn1.ASN1InputStream;
import org.bouncycastle.asn1.ASN1Object;
import org.bouncycastle.asn1.ASN1ObjectIdentifier;
import org.bouncycastle.asn1.ASN1OctetString;
import org.bouncycastle.asn1.ASN1Primitive;
import org.bouncycastle.asn1.ASN1Set;
import org.bouncycastle.asn1.BERConstructedOctetString;
import org.bouncycastle.asn1.BEROctetString;
import org.bouncycastle.asn1.BERSet;
import org.bouncycastle.asn1.DERNull;
import org.bouncycastle.asn1.DERObjectIdentifier;
import org.bouncycastle.asn1.DEROctetString;
import org.bouncycastle.asn1.DERSet;
import org.bouncycastle.asn1.cms.AttributeTable;
import org.bouncycastle.asn1.cms.CMSAttributes;
import org.bouncycastle.asn1.cms.CMSObjectIdentifiers;
import org.bouncycastle.asn1.cms.ContentInfo;
import org.bouncycastle.asn1.cms.IssuerAndSerialNumber;
import org.bouncycastle.asn1.cms.SignedData;
import org.bouncycastle.asn1.cms.SignerIdentifier;
import org.bouncycastle.asn1.cms.SignerInfo;
import org.bouncycastle.asn1.pkcs.PKCSObjectIdentifiers;
import org.bouncycastle.asn1.x500.X500Name;
import org.bouncycastle.asn1.x509.AlgorithmIdentifier;
import org.bouncycastle.asn1.x509.DigestInfo;
import org.bouncycastle.asn1.x509.TBSCertificateStructure;
import org.bouncycastle.cms.CMSAttributeTableGenerator;
import org.bouncycastle.cms.CMSException;
import org.bouncycastle.cms.CMSProcessable;
import org.bouncycastle.cms.CMSProcessableByteArray;
import org.bouncycastle.cms.CMSSignedData;
import org.bouncycastle.cms.CMSSignedGenerator;
import org.bouncycastle.cms.CMSSignedHelper;
import org.bouncycastle.cms.DefaultSignedAttributeTableGenerator;
import org.bouncycastle.cms.SignerInformation;
import org.bouncycastle.cms.SignerInformationStore;
import org.bouncycastle.cms.SimpleAttributeTableGenerator;
import org.bouncycastle.crypto.Digest;
import org.bouncycastle.crypto.Signer;
import org.bouncycastle.crypto.params.ParametersWithRandom;
import org.bouncycastle.jcajce.provider.asymmetric.rsa.RSAUtil;
import org.bouncycastle.security.DigestUtilities;



public class CmsSignedDataGenWithRsaCsp extends CMSSignedGenerator {
	private static CMSSignedHelper Helper = new CMSSignedHelper();

	ArrayList<SignerInf> signerInfs = new ArrayList<SignerInf>();
   
//	List _certs = new ArrayList();
//    List _crls = new ArrayList();
//	List<SignerInformation> _signers = new ArrayList<SignerInformation>();
//	Dictionary _digests = new Hashtable();
    
    
    public abstract class BaseOutputStream extends OutputStream
    {
		private boolean closed;



		public boolean canRead() { return false; }
        public boolean canSeek() { return false; } 
        public boolean canWrite() {  return !closed; } 
		public void close() { closed = true; }
        public void flush() {}
        public long length() throws Exception { throw new Exception("Not supported"); } 
        
        public long getPosition()throws Exception { throw new Exception("Not supported"); } 
        public long setPosition()throws Exception { throw new Exception("Not supported"); } 
//        public long position()
//        
//        {
//            get { throw new NotSupportedException(); }
//            set { throw new NotSupportedException(); }
//        }
        public int read(byte[] buffer, int offset, int count) throws Exception { throw new Exception("Not supported"); }
        //public long Seek(long offset, SeekOrigin origin) {throw new Exception("Not supported"); }
        public void SetLength(long value) throws Exception { throw new Exception("Not supported"); }

        
        
        public void Write(byte[] buffer, int offset, int count) throws IOException
        {
            assert(buffer != null);
            assert(0 <= offset && offset <= buffer.length);
            assert(count >= 0);
            
            int end = offset + count;

            assert(0 <= end && end <= buffer.length);

            for (int i = offset; i < end; ++i)
            {
                this.write(buffer[i]);
            }
        }

		public void write(byte[] buffer) throws IOException
		{
			write(buffer, 0, buffer.length);
		}
	}

	private class DigOutputStream extends BaseOutputStream
	{
		protected Digest dig;

		protected DigOutputStream(Digest dig)
		{
			this.dig = dig;
		}
	
		public void writeByte(byte b)
		{
			dig.update(b);
		}
	
		public void write(byte[] b, int off, int len)
		{
			dig.update(b, off, len);
		}

		@Override
		public void write(int b) throws IOException {
			 dig.update((byte) b);
		
		}
	}    
	
	private class SignerInf {
        private  CMSSignedGenerator outer;

        //private  AsymmetricKeyParameter key;
        private  PrivateKey key;
        //private  NetCrypto.RSACryptoServiceProvider krProv;
        //private  RSADigestSigner rsae;
        private  Signature rsae;
        private  SignerIdentifier signerIdentifier;
        private  String digestOID;
        private  String encOID;
        private  CMSAttributeTableGenerator sAttr;
        private  CMSAttributeTableGenerator unsAttr;
        private  AttributeTable baseSignedTable;

		//private Object CmsSignedGenerator;

        /// <summary>
        /// Signer is received with crypto provider or pr. key (if the crypto provider == null)
        /// </summary>
        /// <param name="krProv">Crypto provider. if null than use key</param>
        /// <param name="key">Private key</param>
        public SignerInf(
        		CMSSignedGenerator outer,
        		//NetCrypto.RSACryptoServiceProvider krProv,
        		//RSADigestSigner rsae,
                Signature signatureAlgorithm,
                //private  AsymmetricKeyParameter key;
            PrivateKey key,
            SignerIdentifier signerIdentifier,
            String digestOID,
            String encOID,
            CMSAttributeTableGenerator sAttr,
            CMSAttributeTableGenerator unsAttr,
            AttributeTable baseSignedTable)
        {
            this.outer = outer;
            this.key = key;
            this.rsae = signatureAlgorithm;
            this.signerIdentifier = signerIdentifier;
            this.digestOID = digestOID;
            this.encOID = encOID;
            this.sAttr = sAttr;
            this.unsAttr = unsAttr;
            this.baseSignedTable = baseSignedTable;
        }

        private AlgorithmIdentifier DigestAlgorithmID()
        {
            return new AlgorithmIdentifier(new DERObjectIdentifier(digestOID), DERNull.getInstance(null)); 
        }

        private CMSAttributeTableGenerator SignedAttributes()
        {
            return sAttr;
        }

        private CMSAttributeTableGenerator unsignedAttributes()
        {
            return unsAttr;
        }

        public Digest getDigestInstance(
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
       				for (String alias : Helper.getDigestAliases(algorithm))
       				{
       					try { return DigestUtilities.getDigest(alias); }
       					catch (SecurityException se) {}
       				}
       				throw e;
       			}
       		}

        private SignerInfo toSignerInfo(
            DERObjectIdentifier contentType,
            CMSProcessable content,
            SecureRandom random,
            boolean isCounterSignature) throws Exception
        {
            AlgorithmIdentifier digAlgId = DigestAlgorithmID();
            
            String digestName = Helper.getDigestAlgName(digestOID);

            Digest dig = Helper.getDigestInstance(digestName,null);

            String signatureName = digestName + "with" + Helper.getEncryptionAlgName(encOID);
            //ISigner sig moved there where used

            // TODO Optimise the case where more than one signer with same digest
        	DigOutputStream dos = new DigOutputStream(dig);
            if (content != null)
            {
                content.write(dos);
            }
            
            byte[] hash = DigestUtilities.DoFinal(dig);
            
            
            if (!outer.getGeneratedDigests().containsKey(digestOID))
            	outer.getGeneratedDigests().put(digestOID, hash.clone());
            
//            if (!outer._digests.Contains(digestOID))
//                outer._digests.Add(digestOID, hash.clone());

            ASN1Set signedAttr = null;
            byte[] tmp;   
            if (sAttr != null)
            {
            	//explicit cast
            	Map parameters = outer.getBaseParameters((ASN1ObjectIdentifier) contentType, digAlgId, hash);

                //					Asn1.Cms.AttributeTable signed = sAttr.GetAttributes(Collections.unmodifiableMap(parameters));
               AttributeTable signed = sAttr.getAttributes(parameters);

                if (isCounterSignature)
                {
                    Hashtable tmpSigned = signed.toHashtable();
                    tmpSigned.remove(CMSAttributes.contentType);
                    signed = new AttributeTable(tmpSigned);
                }

                // TODO Validate proposed signed attributes
                
                signedAttr = outer.getAttributeSet(signed);                             
                
                
                // sig must be composed from the DER encoding.
                tmp = signedAttr.getEncoded();
            }
            else
            {
            	
            	// TODO Use raw signature of the hash value instead
            	ByteArrayOutputStream bOut = new ByteArrayOutputStream();
                if (content != null)
                {
                    content.write(bOut);
                }
                tmp = bOut.toByteArray();
            }

            byte[] sigBytes = null;
            if (rsae != null)
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
//ContentSigner

//            	RSACryptoServiceProvider key = new RSACryptoServiceProvider();
//            	key.FromXmlString(privateCert.PrivateKey.ToXmlString(true));

            	
            	/*
                Digest digProv = Helper.getDigestInstance(digestName, null);
                digProv.update(tmp, 0, tmp.length);
                byte[] hashProv = new byte[digProv.getDigestSize()];
                digProv.doFinal(hashProv, 0);
            	*/

                rsae.update(tmp);
                sigBytes = rsae.sign();

              
                //sigBytes = krProv.SignHash(hashProv, digestOID);

            }
            else
            {
            	
                Signer sig = SignerUtilities.GetSigner(signatureName);//Helper.GetSignatureInstance(signatureName);//was moved
                sig.init(true, new ParametersWithRandom(RSAUtil.generatePrivateKeyParameter((RSAPrivateKey)key), random));
                sig.update(tmp, 0, tmp.length);
                sigBytes = sig.generateSignature();
            }

            ASN1Set unsignedAttr = null;
            if (unsAttr != null)
            {
                Map baseParameters = outer.getBaseParameters((ASN1ObjectIdentifier) contentType, digAlgId, hash);
                baseParameters.put(CMSAttributeTableGenerator.SIGNATURE,sigBytes.clone());

                //					Asn1.Cms.AttributeTable unsigned = unsAttr.GetAttributes(Collections.unmodifiableMap(baseParameters));
                AttributeTable unsigned = unsAttr.getAttributes(baseParameters);

                // TODO Validate proposed unsigned attributes

                unsignedAttr = outer.getAttributeSet(unsigned);
            }
            																																																																																																
            // TODO [RSAPSS] Need the ability to specify non-default parameters
            ASN1Encodable sigX509Parameters = SignerUtilities.GetDefaultX509Parameters(signatureName);
            AlgorithmIdentifier encAlgId = CMSSignedGenerator.GetEncAlgorithmIdentifier(
                new DERObjectIdentifier(encOID), sigX509Parameters);

            return new SignerInfo(signerIdentifier, digAlgId,
                signedAttr, encAlgId, new DEROctetString(sigBytes), unsignedAttr);
        }

}

 
    public CmsSignedDataGenWithRsaCsp(){}
    
    /// <summary>Constructor allowing specific source of randomness</summary>
    /// <param name="rand">Instance of <c>SecureRandom</c> to use.</param>
    public CmsSignedDataGenWithRsaCsp(
        SecureRandom rand)
        {
    	
    	super(rand);
        
    }

    public void myAddSigner//NetCrypto.RSACryptoServiceProvider crProv,
    (       
    	Signature signatureAlgorithm,
    	//RSADigestSigner rsa,
        X509Certificate cert,
    	//org.bouncycastle.asn1.x509.Certificate cert,
        String encryptionOID,
        String digestOID,
        AttributeTable signedAttr,
        AttributeTable unsignedAttr
    )
{

    signerInfs.add(new SignerInf(this, signatureAlgorithm, /*crProv,*/ null, getSignerIdentifier(cert), digestOID, encryptionOID,
new DefaultSignedAttributeTableGenerator(signedAttr), new SimpleAttributeTableGenerator(unsignedAttr), null));

}

private SignerIdentifier getSignerIdentifier(
		X509Certificate cert
		//org.bouncycastle.asn1.x509.Certificate cert
		) {
		// TODO Auto-generated method stub
		//X500Name NomeCertificato = new X500Name(cert.getIssuerX500Principal().getName());
		//X500Name NomeCertificato1 = new X500Name(cert.getIssuerDN().getName());
		
		//X500Name NomeCertificato2 = new X500Principal(cert.getIssuerX500Principal().getName(DIGEST_SHA256));
	
		//TBSCertificateStructure tbsCert = TBSCertificateStructure.getInstance(new ASN1InputStream(cert.getTBSCertificate() ));

		//IssuerAndSerialNumber isn = new IssuerAndSerialNumber(new X500Name(cert.getIssuerX500Principal().getName()), cert.getSerialNumber());
		SignerIdentifier tempSignId = null;
	
    	org.bouncycastle.asn1.x509.Certificate c;
		try {
			c = org.bouncycastle.asn1.x509.Certificate.getInstance(ASN1Primitive.fromByteArray(cert.getEncoded()));
			IssuerAndSerialNumber isn = new  IssuerAndSerialNumber(c);
			tempSignId = new SignerIdentifier(isn);
		} catch (CertificateEncodingException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} 
		
		return tempSignId;
	}

private SignerIdentifier getSignerIdentifier(byte[] subjectKeyIdentifier)
{
	return new SignerIdentifier(new DEROctetString(subjectKeyIdentifier));    
}

//public void addSigner(
//    //NetCrypto.RSACryptoServiceProvider crProv,
//    PrivateKey privateKey,
//		X509Certificate cert,
//    String digestOID)
//{
//    addSigner(privateKey,//crProv, 
//    		cert, getEncOid(privateKey,//crProv, 
//    				digestOID), digestOID);
//}

public void addSigner(PrivateKey privateKey,
    //NetCrypto.RSACryptoServiceProvider crProv,
    X509Certificate cert,
    //org.bouncycastle.asn1.x509.Certificate cert,
    String encryptionOID,
    String digestOID)
{
    signerInfs.add(new SignerInf(this, null,//crProv, 
    		null, getSignerIdentifier(cert), digestOID, encryptionOID, new DefaultSignedAttributeTableGenerator(), null, null));
}


public void addSigner(
    byte[] subjectKeyID,
    String encryptionOID,
    String digestOID,
    CMSAttributeTableGenerator signedAttrGen,
    CMSAttributeTableGenerator unsignedAttrGen)
{
    signerInfs.add(new SignerInf(this,null, null, getSignerIdentifier(subjectKeyID),
        digestOID, encryptionOID, signedAttrGen, unsignedAttrGen, null));
}



public void addSigner(
    PrivateKey privateKey,
    X509Certificate cert,
    String digestOID)
{
    addSigner(privateKey, cert, getEncOID(privateKey, digestOID), digestOID);
}


public void AddSigner(
		 PrivateKey privateKey,
		   X509Certificate cert,
    String encryptionOID,
    String digestOID)
{
    signerInfs.add(new SignerInf(this, null, privateKey, getSignerIdentifier(cert), digestOID, encryptionOID,
        new DefaultSignedAttributeTableGenerator(), null, null));
}

/**
 * add a signer - no attributes other than the default ones will be
 * provided here.
 */
public void addSigner(
	PrivateKey privateKey,
    byte[] subjectKeyID,
    String digestOID)
{
    addSigner(privateKey, subjectKeyID, getEncOID(privateKey, digestOID), digestOID);
}

public void addSigner(
		PrivateKey privateKey,
        byte[] subjectKeyID,
        String encryptionOID,
        String digestOID)
    {
        signerInfs.add(new SignerInf(this, null, privateKey, getSignerIdentifier(subjectKeyID),
            digestOID, encryptionOID,
            new DefaultSignedAttributeTableGenerator(), null, null));
    }

public void AddSigner(
	PrivateKey privateKey,
    byte[] subjectKeyID,
    String encryptionOID,
    String digestOID)
{
    signerInfs.add(new SignerInf(this, null, privateKey, getSignerIdentifier(subjectKeyID),
        digestOID, encryptionOID,
        new DefaultSignedAttributeTableGenerator(), null, null));
}


public void addSigner(
	PrivateKey privateKey,
    X509Certificate cert,
    String digestOID,
    AttributeTable signedAttr,
    AttributeTable unsignedAttr)
{
    addSigner(privateKey, cert, getEncOid(privateKey, digestOID), digestOID,
        signedAttr, unsignedAttr);
}


public void addSigner(
    PrivateKey privateKey,
    X509Certificate cert,
    String encryptionOID,
    String digestOID,
    AttributeTable signedAttr,
    AttributeTable unsignedAttr)
{
    signerInfs.add(new SignerInf(this, null, privateKey, getSignerIdentifier(cert),
        digestOID, encryptionOID,
        new DefaultSignedAttributeTableGenerator(signedAttr),
        new SimpleAttributeTableGenerator(unsignedAttr),
        signedAttr));
}


public void addSigner(
	PrivateKey privateKey,
    byte[] subjectKeyID,
    String digestOID,
    AttributeTable signedAttr,
    AttributeTable unsignedAttr)
{
    addSigner(privateKey, subjectKeyID, digestOID, getEncOid(privateKey, digestOID),
        new DefaultSignedAttributeTableGenerator(signedAttr),
        new SimpleAttributeTableGenerator(unsignedAttr));
}

public void addSigner(
		PrivateKey privateKey,
    byte[] subjectKeyID,
    String encryptionOID,
    String digestOID,
    AttributeTable signedAttr,
    AttributeTable unsignedAttr)
{
    signerInfs.add(new SignerInf(this, null, privateKey, getSignerIdentifier(subjectKeyID),
        digestOID, encryptionOID,
        new DefaultSignedAttributeTableGenerator(signedAttr),
        new SimpleAttributeTableGenerator(unsignedAttr),
        signedAttr));
}

/**
 * add a signer with extra signed/unsigned attributes based on generators.
 */
public void addSigner(
		PrivateKey privateKey,
    X509Certificate cert,
    String digestOID,
    CMSAttributeTableGenerator signedAttrGen,
    CMSAttributeTableGenerator unsignedAttrGen)
{
    addSigner(privateKey, cert, getEncOid(privateKey, digestOID), digestOID,
        signedAttrGen, unsignedAttrGen);
}

/**
 * add a signer, specifying the digest encryption algorithm, with extra signed/unsigned attributes based on generators.
 */
public void addSigner(
		PrivateKey privateKey,
    X509Certificate cert,
    String encryptionOID,
    String digestOID,
    CMSAttributeTableGenerator signedAttrGen,
    CMSAttributeTableGenerator unsignedAttrGen)
{
   signerInfs.add(new SignerInf(this, null, privateKey, getSignerIdentifier(cert),
        digestOID, encryptionOID,
        signedAttrGen, unsignedAttrGen, null));
}

public void addSigner(
	//	AsymmetricKeyParameter privateKey,
	PrivateKey privateKey,
	byte[] subjectKeyID,
    String digestOID,
    CMSAttributeTableGenerator signedAttrGen,
    CMSAttributeTableGenerator unsignedAttrGen)
{
    addSigner(privateKey, subjectKeyID, digestOID, getEncOID(privateKey, digestOID),
        signedAttrGen, unsignedAttrGen);
}


public void addSigner(
		//	AsymmetricKeyParameter privateKey,
	PrivateKey privateKey,
    byte[] subjectKeyID,
    String encryptionOID,
    String digestOID,
    CMSAttributeTableGenerator signedAttrGen,
    CMSAttributeTableGenerator unsignedAttrGen)
{

    signerInfs.add(new SignerInf(this, null, privateKey, new SignerIdentifier(new DEROctetString(subjectKeyID)),//GetSignerIdentifier(subjectKeyID),
        digestOID, encryptionOID, signedAttrGen, unsignedAttrGen, null));
}

//public void myAddSigner
//(
//    //NetCrypto.RSACryptoServiceProvider crProv,<<<<<<<<<<<<<<<<<<<<<<<<<<<
//    X509Certificate cert,
//    String encryptionOID,
//    String digestOID,
//    AttributeTable signedAttr,
//    AttributeTable unsignedAttr
//)
//{
////signerInfs.add(new SignerInf(this, crProv, null, GetSignerIdentifier(cert), digestOID, encryptionOID,<<<<<<<<<<<<<<<<<<<<<<
////new DefaultSignedAttributeTableGenerator(signedAttr), new SimpleAttributeTableGenerator(unsignedAttr), null));<<<<<<<<<<<<<<<<<<<<<<<<
//
//}
public CMSSignedData generate(
        String signedContentType,
        CMSProcessable content,
        boolean encapsulate) throws Exception
    {
        ASN1EncodableVector digestAlgs = new ASN1EncodableVector();
        ASN1EncodableVector signerInfos = new ASN1EncodableVector();


        //_digests. = new Hashtable(); // clear the current preserved digest state
        digests.clear();
        
        //
        // add the precalculated SignerInfo objects.
        //
        //for (SignerInformation signer : _signers)
        for (SignerInformation signer : (List<SignerInformation>)_signers)
        {
        	
            digestAlgs.add(Helper.fixAlgID(signer.getDigestAlgorithmID()));
            signerInfos.add(signer.toSignerInfo());
        }

        //
        // add the SignerInfo objects
        //
        boolean isCounterSignature = (signedContentType == null);

        ASN1ObjectIdentifier asn1ContentTypeOID = isCounterSignature ? CMSObjectIdentifiers.data : new ASN1ObjectIdentifier(signedContentType);
        //DERObjectIdentifier derContentTypeOID = isCounterSignature ? CMSObjectIdentifiers.data : new DERObjectIdentifier(signedContentType);
        //DERObjectIdentifier ContentTypeOID = isCounterSignature ? CMSObjectIdentifiers.data : new DERObjectIdentifier(signedContentType);
//        : new DERObjectIdentifier(signedContentType);

        for (SignerInf signer : signerInfs)
        {
            try
            {
                digestAlgs.add(signer.DigestAlgorithmID());
                signerInfos.add(signer.toSignerInfo(asn1ContentTypeOID, content, rand, isCounterSignature));
            }
            catch (IOException e)
            {
                throw new CMSException("encoding error.", e);
            }
            catch (InvalidKeyException e)
            {
                throw new CMSException("key inappropriate for signature.", e);
            }
            catch (SignatureException e)
            {
                throw new CMSException("error creating signature.", e);
            }
            catch (CertificateEncodingException e)
            {
                throw new CMSException("error creating sid.", e);
            }
        }

        ASN1Set certificates = null;

        if (certs.size() != 0)
        {
        	
            certificates = createBerSetFromList(certs);
        }

        ASN1Set certrevlist = null;

        if (crls.size() != 0)
        {
            certrevlist = createBerSetFromList(crls);
        }

        ASN1OctetString octs = null;
        if (encapsulate)
        {
            ByteArrayOutputStream bOut = new ByteArrayOutputStream();
            if (content != null)
            {
                try
                {
                	content.write(bOut);                   
                }
                catch (IOException e)
                {
                    throw new CMSException("encapsulation error.", e);
                }
            }
           // octs = new  BERConstructedOctetString(bOut.toByteArray()); // trento
            octs = new  DEROctetString(bOut.toByteArray());
        }
		
         ContentInfo encInfo = new ContentInfo(asn1ContentTypeOID, octs);
        //ContentInfo encInfo = new ContentInfo(contentTypeOID, octs);

        SignedData sd = new SignedData(
            new DERSet(digestAlgs),
            encInfo,
            certificates,
            certrevlist,
            new DERSet(signerInfos));

        ContentInfo contentInfo = new ContentInfo(CMSObjectIdentifiers.signedData, sd);
        //ContentInfo contentInfo = new ContentInfo(PKCSObjectIdentifiers.signedData, sd); // trento
        
        return new CMSSignedData(content, contentInfo);
    }


/**
 * generate a signed object that for a CMS Signed Data object
 * @throws Exception 
 */
 public CMSSignedData generate(
     CMSProcessable content) throws Exception
 {
     return generate(content, false);
 }

 /**
 * generate a signed object that for a CMS Signed Data
 * object  - if encapsulate is true a copy
 * of the message will be included in the signature. The content type
 * is set according to the OID represented by the string signedContentType.
 * @throws Exception 
 */
// public CMSSignedData generate(
//     String signedContentType,
//     CMSProcessable content,
//     boolean encapsulate) throws Exception
// {
//     ASN1EncodableVector digestAlgs = new ASN1EncodableVector();
//     ASN1EncodableVector signerInfos = new ASN1EncodableVector();
//
//     _digests =  new Hashtable(); // clear the current preserved digest state
//
//     //
//     // add the precalculated SignerInfo objects.
//     //
//     for (SignerInformation signer : _signers)
//     {
//         digestAlgs.add(Helper.fixAlgID(signer.getDigestAlgorithmID()));
//         signerInfos.add(signer.toSignerInfo());
//     }
//
//     //
//     // add the SignerInfo objects
//     //
//     boolean isCounterSignature = (signedContentType == null);
//
//     ASN1ObjectIdentifier contentTypeOID = isCounterSignature
//         ? CMSObjectIdentifiers.data
//         : new ASN1ObjectIdentifier(signedContentType);
//
//     for(SignerInf signer : signerInfs)
//     {
//         try
//         {
//             digestAlgs.add(signer.DigestAlgorithmID());
//             signerInfos.add(signer.toSignerInfo(contentTypeOID, content, rand, isCounterSignature));
//         }
//         catch (IOException e)
//         {
//             throw new CMSException("encoding error.", e);
//         }
//         catch (InvalidKeyException e)
//         {
//             throw new CMSException("key inappropriate for signature.", e);
//         }
//         catch (SignatureException e)
//         {
//             throw new CMSException("error creating signature.", e);
//         }
//         catch (CertificateEncodingException e)
//         {
//             throw new CMSException("error creating sid.", e);
//         }
//     }
//
//     ASN1Set certificates = null;
//
//     if (_certs.size() != 0)
//     {
//         certificates = createBerSetFromList(_certs);
//     }
//
//     ASN1Set certrevlist = null;
//
//     if (_crls.size() != 0)
//     {
//         certrevlist = createBerSetFromList(_crls);
//     }
//
//     ASN1OctetString octs = null;
//     if (encapsulate)
//     {
//         ByteArrayOutputStream bOut = new ByteArrayOutputStream();
//         if (content != null)
//         {
//             try
//             {
//             	content.write(bOut);                   
//             }
//             catch (IOException e)
//             {
//                 throw new CMSException("encapsulation error.", e);
//             }
//         }
//         octs = new BEROctetString(bOut.toByteArray());
//     }
//
//     ContentInfo encInfo = new ContentInfo(contentTypeOID, octs);
//
//     SignedData sd = new SignedData(
//         new DERSet(digestAlgs),
//         encInfo,
//         certificates,
//         certrevlist,
//         new DERSet(signerInfos));
//
//     ContentInfo contentInfo = new ContentInfo(CMSObjectIdentifiers.signedData, sd);
//
//     return new CMSSignedData(content, contentInfo);
// }

 /**
 * generate a signed object that for a CMS Signed Data
 * object - if encapsulate is true a copy
 * of the message will be included in the signature with the
 * default content type "data".
 * @throws Exception 
 */
 public CMSSignedData generate(
     CMSProcessable content,
     boolean encapsulate) throws Exception
 {
     return this.generate(DATA, content, encapsulate);
 }

 /**
 * generate a set of one or more SignerInformation objects representing counter signatures on
 * the passed in SignerInformation object.
 *
 * @param signer the signer to be countersigned
 * @param sigProvider the provider to be used for counter signing.
 * @return a store containing the signers.
 * @throws Exception 
 */
 public SignerInformationStore GenerateCounterSigners(
     SignerInformation signer) throws Exception
 {
     return this.generate(null, new CMSProcessableByteArray(signer.getSignature()), false).getSignerInfos();
 }


 private static ASN1Set createBerSetFromList(
		List<ASN1Encodable> berObjects)
	{
		ASN1EncodableVector v = new ASN1EncodableVector();

		for (ASN1Encodable ae : berObjects)
		{
			v.add(ae);
		}

		return new BERSet(v);
	}
   
 protected String getEncOid(
         PrivateKey privateKey,
         String digestOID)
     {
         String encOID = null;

         //if (crProv is NetCrypto.RSACryptoServiceProvider)
         if (privateKey.getAlgorithm().contains("RSA"))
         {
//             if ((crProv).PublicOnly)
//                 throw new ArgumentException("Expected RSA private key");

             encOID = ENCRYPTION_RSA;
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
}
