package com.nttdata.signapplet.gui;

//import it.integra.signapplet.gui.SignApplet.CertificateInfo;

import java.io.IOException;
import java.security.GeneralSecurityException;
import java.security.KeyStore;
import java.security.KeyStore.Entry;
import java.security.KeyStore.PrivateKeyEntry;
import java.security.KeyStore.ProtectionParameter;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.security.PrivateKey;
import java.security.Security;
import java.security.Signature;
import java.security.cert.Certificate;
import java.security.cert.CertificateEncodingException;
import java.security.cert.X509Certificate;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Enumeration;
import java.util.Iterator;
import java.util.List;

import org.bouncycastle.asn1.ASN1EncodableVector;
import org.bouncycastle.asn1.ASN1Primitive;
import org.bouncycastle.asn1.ASN1Sequence;
import org.bouncycastle.asn1.DERObjectIdentifier;
import org.bouncycastle.asn1.DERSet;
import org.bouncycastle.asn1.cms.Attribute;
import org.bouncycastle.asn1.cms.AttributeTable;
import org.bouncycastle.asn1.cms.SignedData;
import org.bouncycastle.asn1.ess.ESSCertIDv2;
import org.bouncycastle.asn1.ess.SigningCertificateV2;
import org.bouncycastle.asn1.pkcs.PKCSObjectIdentifiers;
import org.bouncycastle.asn1.x509.AlgorithmIdentifier;
import org.bouncycastle.cert.X509CertificateHolder;
import org.bouncycastle.cms.CMSException;
import org.bouncycastle.cms.CMSProcessableByteArray;
import org.bouncycastle.cms.CMSSignedData;
import org.bouncycastle.cms.CMSSignedDataParser;
import org.bouncycastle.cms.CMSSignedGenerator;
import org.bouncycastle.cms.SignerInformation;
import org.bouncycastle.cms.SignerInformationStore;
import org.bouncycastle.cms.jcajce.JcaSimpleSignerInfoVerifierBuilder;
import org.bouncycastle.jce.provider.BouncyCastleProvider;
import org.bouncycastle.operator.jcajce.JcaDigestCalculatorProviderBuilder;
import org.bouncycastle.security.DigestUtilities;
import org.bouncycastle.util.Store;
import org.bouncycastle.x509.X509CollectionStoreParameters;
import org.bouncycastle.x509.X509Store;

import sun.security.mscapi.SunMSCAPI;

public class DigitalSignServices {
	/// <summary>
    /// 
    /// </summary>
	final int KeyUsage_nonRepudiation = 1;

	public boolean checkCertificate = false;

	/// <summary>
    /// 
    /// </summary>
    /// <param name="storeLocation"></param>
    /// <param name="storeName"></param>
    /// <returns></returns>
    public CertificateInfo[] getCertificateList(String storeLocation, String storeName) throws GeneralSecurityException, IOException
    {
    	List<CertificateInfo> certificateCollection;

        if (this.checkCertificate)
        {
            certificateCollection = this.getAllValidCertificates(storeName, storeLocation);
        }
        else
        {
            certificateCollection = this.getAllCertificates(storeName, storeLocation, false);
        }

        List<CertificateInfo> list = new ArrayList<CertificateInfo>();

        for (CertificateInfo certificateTemp : certificateCollection)
        {

            boolean isfirma = false;
            //foreach (X509Extension extension in certificateTemp.Extensions){
            
//            byte[] extension = certificateTemp.xc.getExtensionValue("2.5.29.15");
//            KeyUsageExtension ku = new KeyUsageExtension(extension);
//            ku.getBits()[KeyUsage.nonRepudiation];
            boolean nonRepudiationKey = false;
            if (certificateTemp != null && certificateTemp.xc != null && certificateTemp.xc.getKeyUsage() != null)
            	nonRepudiationKey = certificateTemp.xc.getKeyUsage()[KeyUsage_nonRepudiation];
            
            //for (CertificateInfo extension : certificateTemp.xc.getExtensionValue("2.5.29.15")){
                        //if (extension.Oid.Value.Contains("2.5.29.15"))
//                {
                        	
//                    X509KeyUsageExtension keyUsage = ((System.Security.Cryptography.X509Certificates.X509KeyUsageExtension)(extension));
//                    if (keyUsage.KeyUsages.ToString().ToLower().Contains("nonrepudiation"))
//                    {
                        isfirma = nonRepudiationKey;
//                    }
//                    break;
//                }
//            }

            if (isfirma)
            {

                String algo = certificateTemp.xc.getSigAlgName(); //SignatureAlgorithm.FriendlyName;
                //if (algo.ToLower().Contains("256") && (algo.ToLower().Contains("sha")))
                {
                	list.add(certificateTemp);
//                	new CertificateInfo
//                    {
//                        Archived = certificateTemp.Archived,
//                        DigestAlgorithm = certificateTemp.SignatureAlgorithm.FriendlyName,
//                        IssuerName = certificateTemp.IssuerName.Name,
//                        SerialNumber = certificateTemp.SerialNumber,
//                        Subjecioooolò..òltName = certificateTemp.SubjectName.Name,
//                        ThumbPrint = certificateTemp.Thumbprint,
//                        ValidFromDate = DateTime.Parse(certificateTemp.GetEffectiveDateString()),
//                        ValidToDate = DateTime.Parse(certificateTemp.GetExpirationDateString()),
//                        Version = certificateTemp.Version,
//                    });
                }
            }
        }

        CertificateInfo[] ret = new CertificateInfo[list.size()];
        ret = list.toArray(ret);
        return ret;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="storeName"></param>
    /// <param name="storeLocation"></param>
    /// <returns></returns>
    public String getCertificateListAsJsonFormat(String storeLocation, String storeName) throws GeneralSecurityException, IOException
    {
        String jsonCertificateList = "";

        int contatore = 0;
        jsonCertificateList = "[";

        CertificateInfo[] certList = this.getCertificateList(storeLocation, storeName);

        for (CertificateInfo cert : certList)
        {
            jsonCertificateList = jsonCertificateList + "{";
            jsonCertificateList = jsonCertificateList + "\"Archived\": \"" + cert.Archived + "\", ";
            jsonCertificateList = jsonCertificateList + "\"DigestAlgorithm\": \"" + cert.DigestAlgorithm + "\", ";
            jsonCertificateList = jsonCertificateList + "\"IssuerName\": \"" + cert.IssuerName.replace("\"", "\\\"") + "\", ";
            jsonCertificateList = jsonCertificateList + "\"SerialNumber\": \"" + cert.SerialNumber.replace("\"", "\\\"") + "\", ";
            jsonCertificateList = jsonCertificateList + "\"SubjectName\": \"" + cert.SubjectName.replace("\"", "\\\"") + "\", ";
            jsonCertificateList = jsonCertificateList + "\"ThumbPrint\": \"" + cert.ThumbPrint.replace("\"", "\\\"") + "\", ";
            jsonCertificateList = jsonCertificateList + "\"ValidFromDate\": \"" + cert.ValidFromDate + "\", ";
            jsonCertificateList = jsonCertificateList + "\"ValidToDate\": \"" + cert.ValidToDate + "\", ";
            jsonCertificateList = jsonCertificateList + "\"Version\": \"" + cert.Version + "\"";

            int contatoreTemp = contatore + 1;

            if (contatoreTemp == certList.length)
                jsonCertificateList = jsonCertificateList + "} ";
            else
                jsonCertificateList = jsonCertificateList + "}, ";

            contatore++;
        }

        jsonCertificateList = jsonCertificateList + "]";

        return jsonCertificateList;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    /// <param name="certIndex"></param>
    /// <param name="applyCosign"></param>
    /// <param name="storeLocation"></param>
    /// <param name="storeName"></param>
    /// <returns></returns>
    private PrivateKey getPrivateKey(String alias, //String password, 
    			String storeLocation, String storeName){
 	   PrivateKey pk = null;
 	   
		Security.addProvider(new SunMSCAPI());
		//Security.addProvider(new BouncyCastleProvider());
			
		//WINDOWS-MY opppure WINDOWS-ROOT
		//KeyStore ks = loadKeyStoreLocal("Windows-MY", "SunMSCAPI");
		KeyStore ks;
		try {
			ks = loadKeyStoreLocal("Windows-MY");
	   	    //ks.load(null);
	   	    
	   	    
	   	    PrivateKeyEntry pke;
	   	    KeyStore.PasswordProtection nullPasswordProt = new KeyStore.PasswordProtection(new char[]{});
	   	    pke = (PrivateKeyEntry) ks.getEntry(alias, nullPasswordProt);
	   	    //pke = (PrivateKeyEntry) ks.getEntry(alias, null);	   	
   	    
	   	    //pk = (PrivateKey) ks.getKey(alias, password.toCharArray());
	   	    
	   	    pk = pke.getPrivateKey();
		} catch (GeneralSecurityException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
 	   
 	   return pk;
    }
    
	   private KeyStore loadKeyStoreLocal(String type, String provider)
			   throws GeneralSecurityException, IOException {
			      KeyStore keyStore = KeyStore.getInstance(type, provider);
			      keyStore.load(null, null);

			      return keyStore;
			   }

	   private KeyStore loadKeyStoreLocal(String
			      type)
			   throws GeneralSecurityException, IOException {
			      KeyStore keyStore = KeyStore.getInstance(type);
			      keyStore.load(null, null);

			      return keyStore;
			   }

	   
	   private CertificateInfo findCertificateSN(CertificateInfo[] ciArray, String sn){
		   CertificateInfo ret = null;
		   
			
		   for (CertificateInfo ci : ciArray){
			   if (ci.SerialNumber.equalsIgnoreCase(sn))
				   ret = ci;
		   }
		   
		   return ret;
	   }
	   
       public String signData(byte[] content, int certIndex, boolean applyCosign, String storeLocation, String storeName//, String password
    		   ) throws Exception{
    	   
    	   CertificateInfo[] ciArray = getCertificateList(storeLocation, storeName);
   
    	   CertificateInfo ci = ciArray[certIndex];    	       
    	   
    	   PrivateKey pk = getPrivateKey(ci.alias, //password, 
    			   							storeLocation, storeName);
    	   byte[] signedBytes = SignWithBouncyCastle(content, ci.xc, applyCosign, pk);
    	   String signedDocument = Base64.encode(signedBytes);
			
    	   return signedDocument;
       }

       public String signData(byte[] content, String serialNumber, boolean applyCosign, String storeLocation, String storeName//, String password
    		   ) throws Exception{
    	   
    	   CertificateInfo[] ciArray = getCertificateList(storeLocation, storeName);
   
    	   CertificateInfo ci = findCertificateSN(ciArray, serialNumber);    	       
    	   
    	   PrivateKey pk = getPrivateKey(ci.alias, //password, 
    			   							storeLocation, storeName);
    	   byte[] signedBytes = SignWithBouncyCastle(content, ci.xc, applyCosign, pk);
    	   String signedDocument = Base64.encode(signedBytes);
			
    	   return signedDocument;
       }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="storeName"></param>
    /// <param name="storeLocation"></param>
    /// <returns></returns>
    private List<CertificateInfo> getAllCertificates(String storeName, String storeLocation, boolean valid) throws GeneralSecurityException, IOException
    {
		Security.addProvider(new SunMSCAPI());
		Security.addProvider(new BouncyCastleProvider());

    	KeyStore ks = javaSecurityOpenKeyStore(storeName, storeLocation);

		List<CertificateInfo> cInfoList = new ArrayList<CertificateInfo>();
       
        try
        {
    		Enumeration <String> aliases = ks.aliases();
    		String alias = null;
    		Certificate cert;
    		ProtectionParameter pp = null;
    		Entry en;
    		CertificateInfo ci;
    		
    		while (aliases.hasMoreElements()){
    			try{
    			alias = aliases.nextElement();
//    			if (ks.isCertificateEntry(alias)){
    				cert = ks.getCertificate(alias);
    	        	
    				X509Certificate oPublicCertificate = (X509Certificate) ks.getCertificate(alias);   
    	        	
    	        	if (!valid || (oPublicCertificate.getKeyUsage() !=null && oPublicCertificate.getKeyUsage()[KeyUsage_nonRepudiation])){
	    	        	ci = new CertificateInfo(oPublicCertificate, alias);
	    	        	
	    	        	cInfoList.add(ci);
    	        	}
    			}
    			catch (GeneralSecurityException e) {
    				// TODO Auto-generated catch block
    				System.out.println(e.getMessage() + " " + alias);
    			} 
    		}
        }
        catch(Exception e){
        	System.out.println(e.getMessage());
        }
//        finally
//        {
//            store.Close();
//        }

        return cInfoList;
    }    

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cert"></param>
    /// <returns></returns>
    protected boolean ValidateCertificate(X509Certificate cert)
    {
//        X509Chain chain = new X509Chain();
//
//        // check entire chain for revocation
//        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
//        //DocsPaUtils.LogsManagement.Debugger.Write(String.Format("SignedDocument.CheckCertificate - RevocationFlag: {0}", chain.ChainPolicy.RevocationFlag));
//
//        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
//
//        //DocsPaUtils.LogsManagement.Debugger.Write(String.Format("SignedDocument.CheckCertificate - RevocationMode: {0}", chain.ChainPolicy.RevocationMode));
//
//        // timeout for online revocation list
//        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 0, 5);
//        //DocsPaUtils.LogsManagement.Debugger.Write(String.Format("SignedDocument.CheckCertificate - UrlRetrievalTimeout: {0}", chain.ChainPolicy.UrlRetrievalTimeout));
//
//        // no exceptions, check all properties
//        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
//        //DocsPaUtils.LogsManagement.Debugger.Write(String.Format("SignedDocument.CheckCertificate - VerificationFlags: {0}", chain.ChainPolicy.VerificationFlags));
//
//        // modify time of verification
//        //chain.ChainPolicy.VerificationTime = new DateTime(1999, 1, 1);
//
//        chain.Build(cert);
//
//        return (chain.ChainStatus.Length == 0);
    	return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="storeName"></param>
    /// <param name="storeLocation"></param>
    /// <returns></returns>
    //private X509Certificate2Collection GetAllValidCertificates(StoreName storeName, StoreLocation storeLocation)
    
    private KeyStore javaSecurityOpenKeyStore(String storeName, String storeLocation)			   
    		throws GeneralSecurityException, IOException {
    	
    		
	      KeyStore keyStore;
	      
	      if (storeName == null || storeName.trim().isEmpty())
	    	  keyStore = KeyStore.getInstance(storeLocation);//provider);
	      else
	    	  keyStore = KeyStore.getInstance(storeLocation, storeName);
	      
	      keyStore.load(null, null);

	      return keyStore;
	   }

    
    
    private List<CertificateInfo> getAllValidCertificates(String storeName, String storeLocation) throws GeneralSecurityException, IOException
    {
    	return getAllCertificates(storeName, storeLocation, true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="signedMessage"></param>
    /// <param name="content"></param>
    private static void checkSign(byte[] content) throws Exception
    {
        try
        {
            CMSSignedDataParser     sp = new CMSSignedDataParser(new JcaDigestCalculatorProviderBuilder().setProvider("BC").build(), content);

            sp.getSignedContent().drain();

            Store                   certStore = sp.getCertificates();
            SignerInformationStore  signers = sp.getSignerInfos();
            
            Collection              c = signers.getSigners();
            Iterator                it = c.iterator();

            while (it.hasNext())
            {
                SignerInformation   signer = (SignerInformation)it.next();
                Collection          certCollection = certStore.getMatches(signer.getSID());

                Iterator        certIt = certCollection.iterator();
                X509CertificateHolder cert = (X509CertificateHolder)certIt.next();

                System.out.println("verify returns: " + signer.verify(new JcaSimpleSignerInfoVerifierBuilder().setProvider("BC").build(cert)));
            }
        }
        catch (Exception ex)
        {
            throw new Exception(String.format("Verifica del documento firmato fallita: {0}", ex.getMessage()), ex);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="signedData"></param>
    /// <returns></returns>
    public static boolean isSha256(byte[] signedData) throws CMSException
    {
        CMSSignedData cmsSignedData = new CMSSignedData(signedData);
        return (isSha256(cmsSignedData));
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="signedData"></param>
    /// <returns></returns>
    public static boolean isSha256(CMSSignedData cmsSignedData)
    {
        boolean retval = false;
        try
        {


            SignedData da = SignedData.getInstance(cmsSignedData.getContentInfo().getContent().toASN1Primitive().toASN1Object());
            ASN1Sequence pp = (ASN1Sequence) da.getDigestAlgorithms().toArray()[0].toASN1Primitive().toASN1Object();
            DERObjectIdentifier diww =   (DERObjectIdentifier) pp.toArray()[0];
            AlgorithmIdentifier ai = new AlgorithmIdentifier(diww.getId());
            //AlgorithmIdentifier ai = new AlgorithmIdentifier(new Oid(diww.getId()));
            String cert = DigestUtilities.getAlgorithmName(diww);

            if (cert.contains("256") && cert.toUpperCase().contains("SHA"))
            {
                retval = true;
            }             
        } catch (Exception e)
        {
            System.out.println("errore: " + e.getMessage());
        };

        return retval;
    }

    private static byte[] SignWithBouncyCastle(byte[] data, X509Certificate certificate, boolean applyCoSign, PrivateKey pk) throws Exception
    {
        try
        {        	
        	Signature signatureAlgorithm =
        			   Signature.getInstance("SHA256withRSA");
        				
        			   signatureAlgorithm.initSign(pk);
        			
        	CmsSignedDataGenWithRsaCsp cms = new CmsSignedDataGenWithRsaCsp();

            String oidEncryption = CMSSignedGenerator.ENCRYPTION_RSA;//	CryptoConfig.MapNameToOID("RSA");
            String oidDigest = CMSSignedGenerator.DIGEST_SHA256;//CryptoConfig.MapNameToOID("SHA256");           
            
            Signature signer = Signature.getInstance("SHA256WithRSA", "BC");
	    	
            
            
            CMSSignedData sigData = null;

            if (applyCoSign)
            {
                ArrayList certList = new ArrayList();
                CMSSignedData sigDataInput = new CMSSignedData(data);
                
                //Org.BouncyCastle.X509.Store.IX509Store x509Certs = sigDataInput.GetCertificates("Collection");
                Store x509Certs = sigDataInput.getCertificates();
                //org.bouncycastle.x509.X509Store x509Certs = (X509Store) sigDataInput.getCertificates();
                
                System.out.println("x509Certs.hashCode(): " + x509Certs.hashCode());
                
                cms.addSigners(sigDataInput.getSignerInfos());
                cms.addCertificates(x509Certs);
                cms.myAddSigner(signatureAlgorithm, certificate, oidEncryption, oidDigest , HashCertificato(certificate, oidDigest), null);
                X509CertificateHolder xch = new X509CertificateHolder(certificate.getEncoded());
                certList.add(xch);
                
                X509CollectionStoreParameters PP = new X509CollectionStoreParameters(certList);
               
                X509Store st  = null;
                st = X509Store.getInstance("CERTIFICATE/COLLECTION", PP, "BC");

                cms.addCertificates(st);

                CMSProcessableByteArray processableContent = (CMSProcessableByteArray)sigDataInput.getSignedContent();
                byte[] unsignedContent = (byte[])processableContent.getContent();
                sigData = cms.generate(new CMSProcessableByteArray(unsignedContent), true);
                 
            }
            else
            {
         	 
               cms.myAddSigner(signatureAlgorithm, certificate, oidEncryption, oidDigest, HashCertificato(certificate, oidDigest), null);
               
                ArrayList certList = new ArrayList();
                
                X509CertificateHolder xch = new X509CertificateHolder(certificate.getEncoded());
                
                certList.add(xch);
                
                X509CollectionStoreParameters PP = new X509CollectionStoreParameters(certList);
//              
                X509Store st  = null;
             	st = X509Store.getInstance("CERTIFICATE/COLLECTION", PP, "BC");

                cms.addCertificates(st);
                
                              
                sigData = cms.generate(new CMSProcessableByteArray(data), true);
            }

          if (!isSha256(sigData))
                throw new Exception("ERRORE: La firma applicata non è SHA256");

            return sigData.getEncoded();
        }
        catch (Exception ex)
        {
            throw new Exception(String.format("Errore nella firma digitale: {0}", ex.getMessage()), ex);
        }
    }

    public static boolean verifica(byte[] data) throws CMSException
    {

        CmsSignedDataGenWithRsaCsp cms = new CmsSignedDataGenWithRsaCsp();
        boolean retval= false;
      
        CMSSignedData sigDataInput = new CMSSignedData(data);

        Store x509Certs = sigDataInput.getCertificates();// GetCertificates("Collection");

        SignerInformationStore sistore = sigDataInput.getSignerInfos();
       
        for (SignerInformation s : (Collection<SignerInformation>)sistore.getSigners())
        {

        	Collection ccc = x509Certs.getMatches(s.getSID());
        	
            for (Object obj: ccc.toArray())
            {
            	X509CertificateHolder cert = (X509CertificateHolder) obj;
            	
//                AsymmetricKeyParameter key1 = new AsymetricK c.getPublicKey();
                try
                {
                	retval = s.verify(new JcaSimpleSignerInfoVerifierBuilder().setProvider("BC").build(cert));
//                    retval  = s.verify(key1);
                    if (!retval)
                        break;
                }
                catch(Exception e) {System.out.println(e.getMessage()); };
            }
        }
        return retval;

    }

    private static AttributeTable HashCertificato(
    		X509Certificate certificate, 
    		//org.bouncycastle.asn1.x509.Certificate certificate,
    		String oid) throws NoSuchAlgorithmException, CertificateEncodingException
    {
    	
        //SHA256Managed hashSha256 = new SHA256Managed();
    	MessageDigest md = MessageDigest.getInstance("SHA-256");
    	//md.update(certificate.getEncoded());
    	
    	org.bouncycastle.asn1.x509.Certificate c=null;
    	try {
			c = org.bouncycastle.asn1.x509.Certificate.getInstance(ASN1Primitive.fromByteArray(certificate.getEncoded()));
			md.update(c.getEncoded());
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
    	
    	
    	
    	//md.update(certificate.getEncoded());
    	//md.update(certificate.Rawdata); // Change this to "UTF-16" if needed
    	byte[] certHash = md.digest();
    	
    	//byte[] certHash = hashSha256.ComputeHash(certificate.RawData);
        ESSCertIDv2 essCert = new ESSCertIDv2(new AlgorithmIdentifier(oid), certHash);
        SigningCertificateV2 scertv2 = new SigningCertificateV2(new ESSCertIDv2[] { essCert });
        Attribute CertHashAttribute = new Attribute(PKCSObjectIdentifiers.id_aa_signingCertificateV2, new DERSet(scertv2));
        ASN1EncodableVector vector = new ASN1EncodableVector();
        vector.add(CertHashAttribute);
        AttributeTable attrTable = new AttributeTable(vector);
        return attrTable;
    }

}
