package com.nttdata.signapplet.gui;

import java.applet.Applet;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.RandomAccessFile;
import java.security.GeneralSecurityException;

import javax.swing.JButton;

import org.bouncycastle.cms.CMSException;

public class SignApplet_DEBUG_COSIGN extends Applet {

	private JButton fe = null;
	
	private static final String PKCS11_KEYSTORE_TYPE =
		      "PKCS11";
	
	private static final String SUN_PKCS11_PROVIDER_CLASS =
		      "sun.security.pkcs11.SunPKCS11";

	private static final DigitalSignServices dss = new DigitalSignServices();
	
	
//	private boolean checkCertificate = false;
//	
	public void setCheckCertificate(boolean b) {
		dss.checkCertificate = b;		
	}
	/**
	    * Loads the keystore from the smart card using its PKCS#11
	    * implementation library and the Sun PKCS#11 security provider.
	    * The PIN code for accessing the smart card is required.
	    */


    private static boolean saveBinaryData(String filePath, String base64Content) throws IOException{
        final String _path = filePath;
        final byte[] decoded = Base64.decode(base64Content.toCharArray());
        	Boolean b = java.security.AccessController.doPrivileged(
    			    new java.security.PrivilegedAction<Boolean>() {
    			        public Boolean run() {
    			        	FileOutputStream  fos = null;
    			        	Boolean ret = false;
    			        	try{
    				        	fos = new FileOutputStream(_path);
    				        	fos.write(decoded);
    				        	fos.close();
    				        	ret = true;
    			        	}
    			        	catch (Exception e){e.printStackTrace();}
    			        	finally{if (fos!=null)
    							try {
    								fos.close();
    							} catch (IOException e) {
    								e.printStackTrace();
    							}}
    						return ret;
    			        }
    			    }
    			 );
    		return b.booleanValue();
    	}
    private static byte[] loadBinaryData(String filePath) throws IOException{
        final String _path = filePath;
        	byte[] data = java.security.AccessController.doPrivileged(
    			    new java.security.PrivilegedAction<byte[]>() {
    			        public byte[] run() {
    			        	FileOutputStream  fos = null;
    			        	byte[] outData = null;
    			        	try{
    			        		
    							RandomAccessFile f = new RandomAccessFile(_path, "r");
    							outData = new byte[(int)f.length()];
    							f.read(outData);
    							f.close();
    							
    			        	}
    			        	catch (Exception e){e.printStackTrace();}
    			        	finally{if (fos!=null)
    							try {
    								fos.close();
    							} catch (IOException e) {
    								e.printStackTrace();
    							}}
    			        	return outData;
    			        }
    			    }
    			 );
    		return data;
    	}

	    
	    public void test(String pathIn, String pathOut) {
			   try {
					CertificateInfo[] cia = getCertificateList("Windows-MY", null);
					String jsonS = getCertificateListAsJsonFormat("Windows-MY", null);
					
					//byte[] data = {-1, -2, 49, 0, 50, 0, 51, 0, 52, 0, 53, 0, 54, 0, 55, 0, 56, 0, 57, 0, 48, 0};
					//String signedData = dss.signData(data, cia[0].SerialNumber, false, "Windows-MY", null);
					
					byte[] data = loadBinaryData(pathIn);

					int[] intData = new int[data.length];
					
					for (int i =0; i<intData.length;i++)
						intData[i] = (int) data[i];
					String signedData = signData(intData, 0, true, "Windows-MY", null);
					
					byte[] signedContent = Base64.decode(signedData.toCharArray());
					
					
					saveBinaryData(pathOut, signedData);
//					FileOutputStream fos = new FileOutputStream("c:\temp\text.p7m");
//				    fos.write(signedContent);
//				    fos.close();
//				    Writer output = null;
//					  String text = "Rajesh Kumar";
//					  File file = new File("c:\temp\text.p7m");
//					  output = new java.io.BufferedWriter(new java.io.FileWriter(file));
//					  output.write(text);
//					  output.close();
		 
					intData = new int[signedContent.length];
							
					for (int i =0; i<intData.length;i++)
						intData[i] = (int) signedContent[i];
							
		            boolean b = verifica(intData);
					System.out.println(signedData);
					System.out.println(b);
				} catch (GeneralSecurityException ex) {
					// TODO Auto-generated catch block
					ex.printStackTrace();
				} catch (IOException ex) {
					// TODO Auto-generated catch block
					ex.printStackTrace();
				} catch (Exception ex) {
					// TODO Auto-generated catch block
					ex.printStackTrace();
				}
    	
	    }

	    
	    public void init() {
		   
				fe = new JButton("DoIt");
				fe.addActionListener(new ActionListener() {
					public void actionPerformed(ActionEvent e) {
						test("c:\\temp\\text.p7m", "c:\\temp\\Cofirma_text.p7m");
					}
				});
						   
				this.add(fe);
				
			   
//			   dss.checkCertificate = true;
//			   String xml = dss.getCertificateListAsJsonFormat("Windows-MY", null);
//			    System.out.println("check:" + Boolean.toString(dss.checkCertificate) + "---" + xml);
//
//			   dss.checkCertificate = false;
//			    xml = dss.getCertificateListAsJsonFormat("Windows-MY", null);
//			    
//			    System.out.println("check:" + Boolean.toString(dss.checkCertificate) + "---" + xml);
//} catch (GeneralSecurityException e) {
//			// TODO Auto-generated catch block
//			e.printStackTrace();
//		} catch (IOException e) {
//			// TODO Auto-generated catch block
//			e.printStackTrace();
//		}
//		   
////		   DigitalSignServices_ dss = new DigitalSignServices_();
////		   
////		   
//		   try {
//			CertificateInfo[] cia = dss.getCertificateList("Windows-MY", null);
//			String jsonS = dss.getCertificateListAsJsonFormat("Windows-MY", null);
//			
//			byte[] data = {49,50,51,52,53,54,55,56,57,48};
//			//String signedData = dss.signData(data, cia[0].SerialNumber, false, "Windows-MY", null);
//			String signedData = dss.signData(data, 0, false, "Windows-MY", null);
//			
//			byte[] signedContent = Base64.decode(signedData.toCharArray());
//			
//			saveBinaryData("c:\\temp\\text.p7m", signedData);
////			FileOutputStream fos = new FileOutputStream("c:\temp\text.p7m");
////		    fos.write(signedContent);
////		    fos.close();
////		    Writer output = null;
////			  String text = "Rajesh Kumar";
////			  File file = new File("c:\temp\text.p7m");
////			  output = new java.io.BufferedWriter(new java.io.FileWriter(file));
////			  output.write(text);
////			  output.close();
// 
//			int[] intData = new int[signedContent.length];
//					
//			for (int i =0; i<intData.length;i++)
//				intData[i] = (int) signedContent[i];
//					
//            boolean b = verifica(intData);
//			System.out.println(signedData);
//		} catch (GeneralSecurityException e) {
//			// TODO Auto-generated catch block
//			e.printStackTrace();
//		} catch (IOException e) {
//			// TODO Auto-generated catch block
//			e.printStackTrace();
//		} catch (Exception e) {
//			// TODO Auto-generated catch block
//			e.printStackTrace();
//		}
//	   
//
	}

	   
       public CertificateInfo[] getCertificateList(final String storeLocation, final String storeName) throws GeneralSecurityException, IOException{
    	   //DigitalSignServices_ dss = new DigitalSignServices_();
    	   CertificateInfo[] list = java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<CertificateInfo[]>() {
			        public CertificateInfo[] run() {
			        	CertificateInfo[] ret = null;
			        	try{
			        		ret = dss.getCertificateList(storeLocation, storeName);
			        	}
			        	catch (Exception e){e.printStackTrace();}
						return ret;
			        }
			    }
			    
    		);
    	   return list;	 
       }
    	   //return dss.getCertificateListAsJsonFormat(storeLocation, storeName);
       public String getCertificateListAsJsonFormat(final String storeLocation, final String storeName) throws GeneralSecurityException, IOException{
    	   //DigitalSignServices_ dss = new DigitalSignServices_();
    	   String list = java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<String>() {
			        public String run() {
			        	String ret = null;
			        	try{
			        		ret = dss.getCertificateListAsJsonFormat(storeLocation, storeName);
			        	}
			        	catch (Exception e){e.printStackTrace();}
						return ret;
			        }
			    }
			    
    		);
    	   return list;	 
       }
    	   //return dss.getCertificateListAsJsonFormat(storeLocation, storeName);
       	
	   
       public String signData(int[] content, final int certIndex, final boolean applyCosign, final String storeLocation, final String storeName) throws Exception{         
    	  final byte[] byteContent = new byte[content.length];
    	   
           for(int i = 0; i<content.length; i++)
        	   byteContent[i] = (byte) content[i];
    	   
    	   String signedData = java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<String>() {
			        public String run() {
			        	String ret = null;
			        	try{
			        		ret = dss.signData(byteContent, certIndex, applyCosign, storeLocation, storeName);
			        	}
			        	catch (Exception e){e.printStackTrace();}
						return ret;
			        }
			    }
			    
    		);
    	   return signedData;	 
       }

       public String signData(int[] content, final String serialNumber, final boolean applyCosign, final String storeLocation, final String storeName) throws Exception{         
    	  final byte[] byteContent = new byte[content.length];
    	   
           for(int i = 0; i<content.length; i++)
        	   byteContent[i] = (byte) content[i];
    	   
    	   String signedData = java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<String>() {
			        public String run() {
			        	String ret = null;
			        	try{
			        		ret = dss.signData(byteContent, serialNumber, applyCosign, storeLocation, storeName);
			        	}
			        	catch (Exception e){e.printStackTrace();}
						return ret;
			        }
			    }
			    
    		);
    	   return signedData;	 
       }
       
       public static boolean isSha256(int[] signedData) throws CMSException
       {
     	  final byte[] byteContent = new byte[signedData.length];
   	   
          for(int i = 0; i<signedData.length; i++)
       	   byteContent[i] = (byte) signedData[i];
   	   
	   	   Boolean verify = java.security.AccessController.doPrivileged(
				    new java.security.PrivilegedAction<Boolean>() {
				        public Boolean run() {
				        	Boolean ret = null;
				        	try{
	    	   ret =  DigitalSignServices.isSha256(byteContent);
				        	}
				        	catch (Exception e){e.printStackTrace();}
							return ret;
				        }
				    }
				    
	    		);
    	   return verify.booleanValue();	 
       }

    
       public boolean verifica(int[] data) throws CMSException{
    	   final byte[] byteData = new byte[data.length];
    	   
           for(int i = 0; i<data.length; i++)
        	   byteData[i] = (byte) data[i];
    	   
	   	   Boolean verify = java.security.AccessController.doPrivileged(
				    new java.security.PrivilegedAction<Boolean>() {
				        public Boolean run() {
				        	Boolean ret = null;
				        	try{
	    	   ret =  DigitalSignServices.verifica(byteData);
				        	}
				        	catch (Exception e){e.printStackTrace();}
							return ret;
				        }
				    }
				    
	    		);
    	   return verify.booleanValue();	 
  }
       
       
       
       
 boolean testSHA256(){
 boolean ret = false;
 
 return ret;
 }
//       public String SignData(byte[] content, int certIndex, boolean applyCosign, String storeLocation, String storeName, String password) throws Exception
//       {
//           String signedData = null;
//           //DigitalSignServices_ signService = new DigitalSignServices_();
//           //signService.CheckCertificate = CheckCertificate;
//           signedData = signData(content, certIndex, applyCosign, storeLocation, storeName, password);
//           return signedData;
//       }
//
//       
//       public CertificateInfo[] GetCertificateList(String storeLocation, String storeName)
//       {
//    	   CertificateInfo[] cInfoArray = null;
//    	   
//    	   ArrayList<CertificateInfo> cInfoList = new ArrayList<CertificateInfo>();
//    	   CertificateInfo ci = null;
//    	   
//    	   try {
//    			
//    			//SunMSCAPI smsc = new SunMSCAPI(); 
//    		Security.addProvider(new SunMSCAPI());
//    		Security.addProvider(new BouncyCastleProvider());
//    			
//    		//WINDOWS-MY opppure WINDOWS-ROOT
//    		//KeyStore ks = loadKeyStoreLocal("Windows-MY", "SunMSCAPI");
//    		KeyStore ks = loadKeyStoreLocal("Windows-MY");
//    	    ks.load(null, null);
//    		
//    	    
//    		Enumeration <String> aliases = ks.aliases();
//    		String alias;
//    		Certificate cert;
//    		ProtectionParameter pp = null;
//    		Entry en;
//    		
//    		while (aliases.hasMoreElements()){
//    			try{
//    			alias = aliases.nextElement();
//    			if (ks.isCertificateEntry(alias)){
//    				cert = ks.getCertificate(alias);
//    	        	
//    				X509Certificate oPublicCertificate = (X509Certificate) ks.getCertificate(alias);   
//    	        	X509CertificateHolder x509Cert = new X509CertificateHolder(oPublicCertificate.getEncoded());
//    	        	
//    	        	List<String> eku = oPublicCertificate.getExtendedKeyUsage();
//    	        	
//    	        	ci = new CertificateInfo(oPublicCertificate, alias);
//    	        	
//    	        	cInfoList.add(ci);
//    	        	
////    	        	ci.PrivateKey = 
////    	        		       Key key=keyStore.getKey("brian", "privatekeypass".toCharArray());
////    	            System.out.println("Key information " + key.getAlgorithm() + " " 
////    	            + key.getFormat());
//
//    	        	
//    	        	//x509Cert.isSignatureValid()
//
//    			}
//    				if (ks.isKeyEntry(alias))
//    			
//    				en = ks.getEntry(alias, pp);
//    			}
//    			catch (GeneralSecurityException e) {
//    				// TODO Auto-generated catch block
//    				e.printStackTrace();
//    			} 
//    		}
//    		
//    		cInfoArray = (CertificateInfo[]) cInfoList.toArray();
//    	} catch (GeneralSecurityException e) {
//    		// TODO Auto-generated catch block
//    		e.printStackTrace();
//    	} catch (IOException e) {
//    		// TODO Auto-generated catch block
//    		e.printStackTrace();
//    	}
//  
//    	  
//    	   return cInfoArray;
//
//    	   
//       }
//       
//       
//       
//       public static byte[] signer(byte[] data, PrivateKey key) {
//    	   byte[] ret = null;
//    	    Signature signer = null;
//			try {
//				signer = Signature.getInstance("SHA256WithRSA", "BC");
//    	    	signer.initSign(key);
//	    	    signer.update(data);
//	    	    ret =  signer.sign();
//			} catch (NoSuchAlgorithmException e) {
//				// TODO Auto-generated catch block
//				e.printStackTrace();
//			} catch (NoSuchProviderException e) {
//				// TODO Auto-generated catch block
//				e.printStackTrace();
//			} catch (InvalidKeyException e) {
//				// TODO Auto-generated catch block
//				e.printStackTrace();
//			} catch (SignatureException e) {
//				// TODO Auto-generated catch block
//				e.printStackTrace();
//			}
//			
//			return ret;
//    	}
//       
//       private PrivateKey getPrivateKey(String alias, String password, String storeLocation, String storeName){
//    	   PrivateKey pk = null;
//    	   
//   		Security.addProvider(new SunMSCAPI());
//   		//Security.addProvider(new BouncyCastleProvider());
//   			
//   		//WINDOWS-MY opppure WINDOWS-ROOT
//   		//KeyStore ks = loadKeyStoreLocal("Windows-MY", "SunMSCAPI");
//   		KeyStore ks;
//		try {
//			ks = loadKeyStoreLocal("Windows-MY");
//	   	    ks.load(null, password.toCharArray());
//	   	    
//	   	    pk = (PrivateKey) ks.getKey(alias, password.toCharArray());
//		} catch (GeneralSecurityException e) {
//			// TODO Auto-generated catch block
//			e.printStackTrace();
//		} catch (IOException e) {
//			// TODO Auto-generated catch block
//			e.printStackTrace();
//		}
//    	   
//    	   return pk;
//       }
//       
//       private String signData(byte[] content, int certIndex, boolean applyCosign, String storeLocation, String storeName, String password) throws Exception{
//    	   String ret = null;
//   
//    	   CertificateInfo[] ciArray = GetCertificateList(storeLocation, storeName);
//   
//    	   X509Certificate cert = ciArray[certIndex].xc;    	       
//    	   
//    	   PrivateKey pk = getPrivateKey(ciArray[certIndex].alias, password, storeLocation, storeName);
//    	   byte[] signedBytes = SignWithBouncyCastle(content, cert, applyCosign, pk);
//    	   String signedDocument = Base64.encode(signedBytes);
//			
//    	   return signedDocument;
//       }
//       
////       private String OldsignData(byte[] content, int certIndex, boolean applyCosign, String storeLocation, String storeName, boolean checkCertificate)
////       {
////           CertificateInfo[] certificateInfoList = GetCertificateList(storeLocation, storeName);
////           CertificateInfo signerCertificateInfo = certificateInfoList[certIndex];
////
//////           StoreName storeNameAsEnum = (StoreName)Enum.Parse(typeof(StoreName), storeName, true);
//////           StoreLocation storeLocationAsEnum = (StoreLocation)Enum.Parse(typeof(StoreLocation), storeLocation, true);
////
////           X509Store store = new X509Store(storeNameAsEnum, storeLocationAsEnum);
////
////           X509Certificate2Collection certificateCollection;
////           X509Certificate2 cert;
////
////           try
////           {
////               store.Open(OpenFlags.ReadWrite);
////               certificateCollection = store.Certificates.Find(
////                               X509FindType.FindBySerialNumber,
////                               signerCertificateInfo.SerialNumber, false);
////           }
////           finally
////           {
////               store.Close();
////           }
////
////           if (certificateCollection.Count != 1)
////           {
////               return null;
////           }
////
////           cert = certificateCollection[0];
////
////           if (applyCosign)
////           {
////               //checkSign(signedMessage, content);
////           }
////           //X509ChainStatusFlags
////
////           byte[] signedBytes = SignWithBouncyCastle(content, cert, applyCosign);
////           String signedDocument = Convert.ToBase64String(signedBytes);
////
////           return signedDocument;
////       }
//
//       /// <summary>
//       /// 
//       /// </summary>
//       /// <param name="crProv"></param>
//       /// <param name="cert"></param>
//       /// <param name="encryptionOID"></param>
//       /// <param name="digestOID"></param>
//       /// <param name="signedAttr"></param>
//       /// <param name="unsignedAttr"></param>
////       public void myAddSigner
////           (
////               //NetCrypto.RSACryptoServiceProvider crProv,<<<<<<<<<<<<<<<<<<<<<<<<<<<
////               X509Certificate cert,
////               String encryptionOID,
////               String digestOID,
////               AttributeTable signedAttr,
////               AttributeTable unsignedAttr
////           )
////       {
//////           signerInfs.add(new SignerInf(this, crProv, null, GetSignerIdentifier(cert), digestOID, encryptionOID,<<<<<<<<<<<<<<<<<<<<<<
//////   new DefaultSignedAttributeTableGenerator(signedAttr), new SimpleAttributeTableGenerator(unsignedAttr), null));<<<<<<<<<<<<<<<<<<<<<<<<
////
////       }
//       
//       public static byte[] SignWithBouncyCastle(byte[] data, X509Certificate certificate, boolean applyCoSign, PrivateKey pk) throws Exception
//       {
//           try
//           {
////               RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)pk;<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
//
//               
//               //Org.BouncyCastle.X509.X509Certificate certCopy = Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(certificate);
//               
//           		X509CertificateHolder x509Cert = new X509CertificateHolder(certificate.getEncoded());
//           		
//           		CmsSignedDataGenWithRsaCsp cms = new CmsSignedDataGenWithRsaCsp();
//
//         	   
////               CmsSignedDataGenWithRsaCsp cms = new CmsSignedDataGenWithRsaCsp();
////
//           	 
//               String oidEncryption = CMSSignedGenerator.ENCRYPTION_RSA;//	CryptoConfig.MapNameToOID("RSA");
//               String oidDigest = CMSSignedGenerator.DIGEST_SHA256;//CryptoConfig.MapNameToOID("SHA256");
////             
////               
//               
////               Signature signer = Signature.getInstance("SHA256WithRSA", "BC");
//   	    	
//               
//               
//               CMSSignedData sigData = null;
//
//               if (applyCoSign)
//               {
//                   ArrayList certList = new ArrayList();
//                   CMSSignedData sigDataInput = new CMSSignedData(data);
//                   
//                   //Org.BouncyCastle.X509.Store.IX509Store x509Certs = sigDataInput.GetCertificates("Collection");
//                   org.bouncycastle.x509.X509Store x509Certs = (X509Store) sigDataInput.getCertificates();
//                   
//                   
//                   
//                   cms.addSigners(sigDataInput.getSignerInfos());
//                   cms.addCertificates(x509Certs);
////                   cms.MyAddSigner(rsa, certCopy, oidEncryption, oidDigest , HashCertificato(certificate, oidDigest), null);<<<<<<<<<<<<<<<<<<<<<<<
//                   certList.add(x509Cert);
//
//                   
//                   
//                   X509CollectionStoreParameters PP = new X509CollectionStoreParameters(certList);
//                   
//                   
//                   //X509Store st1 = Org.BouncyCastle.X509.Store.X509StoreFactory.Create("CERTIFICATE/COLLECTION", PP);
//                   //X509Util;
////                   X509Util.Implementation xi = X509Util.Implementation();
////                   X509Store st1 =new X509Store(); 
////                		   
////                		   X509Store(null, PP);
//
//                   //X509StoreCertCollection st1 = new X509StoreCertCollection(PP);
//                   X509StoreCertCollection st1 = new X509StoreCertCollection();
//                   
//                   
////                   Store st = Store.getInstance();<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
////                   
//                   st1.engineInit(PP);
////                   
////                   cms.addCertificates(st1.engineGetMatches( new X509CertStoreSelector()));<<<<<<<<<<<<<<<<<<<<<<<<<<<<
//                   CMSProcessableByteArray processableContent = (CMSProcessableByteArray)sigDataInput.getSignedContent();
//                   byte[] unsignedContent = (byte[])processableContent.getContent();
//                   sigData = cms.generate(new CMSProcessableByteArray(unsignedContent), true);
//                    
//               }
//               else
//               {
//            	   
////                   cms.MyAddSigner(rsa, certCopy, oidEncryption, oidDigest, HashCertificato(certificate, oidDigest), null);<<<<<<<<<<<<<<<<<<<<<<<<<<
//
//                   ArrayList certList = new ArrayList();
//                   //certList.Add(certCopy);
//                   certList.add(x509Cert);
//                   
//                   X509CollectionStoreParameters PP = new X509CollectionStoreParameters(certList);
////                   Org.BouncyCastle.X509.Store.IX509Store st1 = Org.BouncyCastle.X509.Store.X509StoreFactory.Create("CERTIFICATE/COLLECTION", PP);<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<>
////                   cms.AddCertificates(st1);<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
//
//                   sigData = cms.generate(new CMSProcessableByteArray(data), true);
//               }
//
//             if (!isSha256(sigData))
//                   throw new Exception("ERRORE: La firma applicata non è SHA256");
//
//               return sigData.getEncoded();
//           }
//           catch (Exception ex)
//           {
//               throw new Exception(String.format("Errore nella firma digitale: {0}", ex.getMessage()), ex);
//           }
//       }
//
//
//       
////http://www.mombu.com/programming/java/t-getting-private-key-from-certificate-536925.html
//       
//       private static boolean isSha256(CMSSignedData sigData) {
//		// TODO Auto-generated method stub
//		return false;
//	}
//
//
//
////	private class CertificateInfo
////       {
////           public boolean Archived;
////           public String DigestAlgorithm;
////
////           public String alias;
////           public String IssuerName;
////           public String PrivateKey;
////           public String SerialNumber;
////           public String SubjectName;
////           public String ThumbPrint;
////           public Date ValidFromDate;
////           public Date ValidToDate;
////           public int Version;
////           public X509Certificate xc;
////           
////           public CertificateInfo(){}
////           
////           public CertificateInfo(X509Certificate xc, String alias){
////        	   try {
////     	   this.IssuerName = xc.getIssuerDN().getName();
////        	   this.SerialNumber = xc.getSerialNumber().toString();
////        	   this.ValidFromDate = xc.getNotBefore();
////        	   this.ValidToDate = xc.getNotAfter();
////        	   this.Version = xc.getVersion();
////        	   //this.Archived = ???
////        	   //this.PrivateKey = ??;
////        	   this.alias = alias;
////        	   this.DigestAlgorithm = xc.getSigAlgName();
////        	   this.SubjectName = xc.getSubjectDN().getName();
////        	   
////        	   MessageDigest md = MessageDigest.getInstance("SHA-1");
////
////        	   this.ThumbPrint = byteArrayToHexString(md.digest(xc.getTBSCertificate()));
////        	   
////        	   this.xc = xc;
////				
////			} catch (CertificateEncodingException e) {
////				// TODO Auto-generated catch block
////				e.printStackTrace();
////			} catch (NoSuchAlgorithmException e) {
////				// TODO Auto-generated catch block
////				e.printStackTrace();
////			}
////           }
////           
////           private String byteArrayToHexString(byte[] b) {
////        	   String result = "";
////        	   for (int i=0; i < b.length; i++) {
////        	     result +=
////        	           Integer.toString( ( b[i] & 0xff ) + 0x100, 16).substring( 1 );
////        	   }
////        	   return result;
////        	 }
////         }
////
//
//    	   
//    public static boolean isSha256(byte[] signedData) throws CMSException
//    {
//        CMSSignedData cmsSignedData = new CMSSignedData(signedData);
//        return (isSha256SD(cmsSignedData));
//    }
//    
//    
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="signedData"></param>
//    /// <returns></returns>
//    public static boolean isSha256SD(CMSSignedData cmsSignedData)
//    {
//        boolean retval = false;
//        try
//        {
//
//
//            SignedData da = SignedData.getInstance(cmsSignedData.getContentInfo().getContent().toASN1Primitive());
//            
//            ASN1Sequence pp = (ASN1Sequence) da.getDigestAlgorithms().getObjectAt(0);
//            DERObjectIdentifier diww = (DERObjectIdentifier) pp.getObjectAt(0);
//            AlgorithmIdentifier ai = new AlgorithmIdentifier(diww.getId());
//            String cert = DigestUtilities.getAlgorithmName(diww);
//
//            if (cert.contains("256") && cert.toUpperCase().contains("SHA"))
//            {
//                retval = true;
//            }
//             
//            
//            /*
//            SignerInformationStore signers = cmsSignedData.GetSignerInfos();
//            ICollection c = signers.GetSigners();
//            IX509Store x509Certs = cmsSignedData.GetCertificates("Collection");
//
//           
//            foreach (SignerInformation signer in c)
//            {
//
//                ICollection certCollection = x509Certs.GetMatches(signer.SignerID);
//                IEnumerator certEnum = certCollection.GetEnumerator();
//
//                certEnum.MoveNext();
//                Org.BouncyCastle.X509.X509Certificate cert = (Org.BouncyCastle.X509.X509Certificate)certEnum.Current;
//                if (cert.SigAlgName.Contains("256") && cert.SigAlgName.ToUpper().Contains("SHA"))
//                {
//                    retval = true;
//                }
//
//            }*/
//        } catch (Exception e)
//        {
//            System.out.println("errore: " + e.getMessage());
//        };
//
//        return retval;
//    }
//
       
}
