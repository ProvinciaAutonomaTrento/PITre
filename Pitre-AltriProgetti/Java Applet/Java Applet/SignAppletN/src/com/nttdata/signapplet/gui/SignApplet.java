package com.nttdata.signapplet.gui;

import java.applet.Applet;
import java.io.FileOutputStream;
import java.io.IOException;
import java.security.GeneralSecurityException;

import org.bouncycastle.cms.CMSException;

public class SignApplet extends Applet {

	/**
	 * 
	 */
	private static final long serialVersionUID = 5071009987192065563L;

	private static final String PKCS11_KEYSTORE_TYPE = "PKCS11";
	
	private static final String SUN_PKCS11_PROVIDER_CLASS = "sun.security.pkcs11.SunPKCS11";

	private static final DigitalSignServices dss = new DigitalSignServices();
	
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

	   public void init() {
		   //try{ dss = new DigitalSignServices();}
		   //catch (Exception e){e.printStackTrace();}
	   }
	   
	   public void close() {
		   this.setEnabled(false);
		   this.destroy();
	   }
	   
	   public void killApplet() 
		 {
			java.security.AccessController.doPrivileged(new java.security.PrivilegedAction<Void>() {
		        public Void run() {
		            // kill the JVM
		            System.exit(0);
		            return null;
		        }
		    });
		 }
	   
       public String getCertificateListAsJsonFormat(final String storeLocation, final String storeName) throws GeneralSecurityException, IOException{
    	   //DigitalSignServices dss = new DigitalSignServices();
    	   //if (dss!=null) {
	    	   String list = java.security.AccessController.doPrivileged(
				    new java.security.PrivilegedAction<String>() {
				        public String run() {
				        	String ret = null;
				        	try{
				        		if (storeName.isEmpty()){
				        			ret = dss.getCertificateListAsJsonFormat(storeLocation, null);
				        		}
				        		else {
				        			ret = dss.getCertificateListAsJsonFormat(storeLocation, storeName);
				        		}
				        	}
				        	catch (Exception e){e.printStackTrace();}
							return ret;
				        }
				    }
				    
	    		);
	    	   return list;
    	   //}
    	   //else
    		   //return null;
       }
    	   //return dss.getCertificateListAsJsonFormat(storeLocation, storeName);
       	
       public String signData(String contentEnc, final int certIndex, final boolean applyCosign, final String storeLocation, final String storeName) throws Exception{         
    	 System.out.println("signData content = string: " + contentEnc);
    	 final byte[] byteContent = Base64.decode( contentEnc.toCharArray());
    	 
  	   		String signedData = java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<String>() {
			        public String run() {
			        	String ret = null;
			        	try{
			        		System.out.println("storeName.isEmpty(): " + Boolean.toString(storeName.isEmpty()));
			        		if (storeName.isEmpty()){
			        			ret = dss.signData(byteContent, certIndex, applyCosign, storeLocation, null);
			        		}
			        		else {
			        			ret = dss.signData(byteContent, certIndex, applyCosign, storeLocation, storeName);
			        		}
			        	}
			        	catch (Exception e){e.printStackTrace();}
						return ret;
			        }
			    }
	  		);
	  	   return signedData;	 
    	}
    	   
       public String signData(String contentEnc, final String serialNumber, final boolean applyCosign, final String storeLocation, final String storeName) throws Exception{         
      	 System.out.println("signData content = string: " + contentEnc);
      	 final byte[] byteContent = Base64.decode(contentEnc.toCharArray());
      	 
    	   		String signedData = java.security.AccessController.doPrivileged(
  			    new java.security.PrivilegedAction<String>() {
  			        public String run() {
  			        	String ret = null;
  			        	try{
  			        		System.out.println("storeName.isEmpty(): " + Boolean.toString(storeName.isEmpty()));
  			        		if (storeName.isEmpty()){
  			        			ret = dss.signData(byteContent, serialNumber, applyCosign, storeLocation, null);
  			        		}
  			        		else {
  			        			ret = dss.signData(byteContent, serialNumber, applyCosign, storeLocation, storeName);
  			        		}
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
             
}
