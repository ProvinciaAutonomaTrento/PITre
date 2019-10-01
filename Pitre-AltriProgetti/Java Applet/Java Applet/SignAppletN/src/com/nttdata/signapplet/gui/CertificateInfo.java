package com.nttdata.signapplet.gui;

import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.security.cert.CertificateEncodingException;
import java.security.cert.X509Certificate;
import java.util.Date;

public class CertificateInfo {
        public boolean Archived;
        public String DigestAlgorithm;

        public String alias;
        public String IssuerName;
        public String PrivateKey;
        public String SerialNumber;
        public String SubjectName;
        public String ThumbPrint;
        public Date ValidFromDate;
        public Date ValidToDate;
        public int Version;
        public X509Certificate xc;
        
        public CertificateInfo(){}
        
        public CertificateInfo(X509Certificate xc, String alias){
     	   try {
  	   this.IssuerName = xc.getIssuerDN().getName();
     	   this.SerialNumber = xc.getSerialNumber().toString();
     	   this.ValidFromDate = xc.getNotBefore();
     	   this.ValidToDate = xc.getNotAfter();
     	   this.Version = xc.getVersion();
     	   //this.Archived = ???
     	   //this.PrivateKey = ??;
     	   this.alias = alias;
     	   this.DigestAlgorithm = xc.getSigAlgName();
     	   this.SubjectName = xc.getSubjectDN().getName();
     	   
     	   MessageDigest md = MessageDigest.getInstance("SHA-1");

     	   this.ThumbPrint = byteArrayToHexString(md.digest(xc.getTBSCertificate()));
     	   
     	   this.xc = xc;
				
			} catch (CertificateEncodingException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (NoSuchAlgorithmException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
        }
        
        private String byteArrayToHexString(byte[] b) {
       	   String result = "";
       	   for (int i=0; i < b.length; i++) {
       	     result +=
       	           Integer.toString( ( b[i] & 0xff ) + 0x100, 16).substring( 1 );
       	   }
       	   return result;
       	 }
        
     }
	