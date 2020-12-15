using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento
{
//	// Subject fields
//	public struct SubjectCommonName
//	{
//		public string CN;
//		public string CodiceFiscale;
//		public string CertId;
//	}

    public enum SignType
    {
        UNKNOWN,
        CADES,
        PADES,
        XADES,

    }

    public enum TsType
    {
        UNKNOWN,
        TSR,
        TSD,
        M7M,
        PADES,
        PKCS,
    }

    [Serializable()]
    public class TSInfo
    {
        public string TSANameIssuer;
        public string TSANameSubject;
        public System.DateTime TSdateTime;
        public string TSimprint;
        public string TSserialNumber;
        public System.DateTime dataFineValiditaCert;
        public System.DateTime dataInizioValiditaCert;
        public TsType TSType;
    }

	// Dati del titolare del certificato
    [Serializable()]
	public struct SubjectInfo
	{
		public string CommonName;
		public string CertId;			// Identificativo del titolare presso il certificatore,
										// per la normativa 2005 è "dnQualifier"

		public string CodiceFiscale;	
		public string DataDiNascita;	// solo per la normativa 2000
        public string Organizzazione;
		public string Ruolo;			// Ruolo del titolare

		public string Cognome;
		public string Nome;
		public string Country;
		public string SerialNumber;		// solo per la normativa 2005: comprende il codice fiscale
	}

	// Certificate information
    [Serializable()]
	public struct CertificateInfo
	{
		public int RevocationStatus;               // Stato revoca (CAPICOM)
		public string RevocationStatusDescription; // Descrizione stato di revoca 
		public string SerialNumber;                // Serial number
        public string SignatureAlgorithm;          // Algoritmo Certificato (OID)
		public DateTime ValidFromDate;             // Valido da
		public DateTime ValidToDate;               // Valido fino a
        public DateTime RevocationDate;             // DataRevoca
		public string SubjectName;                 // Soggetto
		public string IssuerName;                  // Nome della CA emittente
		public string ThumbPrint;                  // Impronta SHA-1
        public byte[] X509Certificate;             // certificato X509
	}
	
	// Signer information
    [Serializable()]
	public struct SignerInfo
	{
		public string ParentSignerCertSN;
		public CertificateInfo CertificateInfo;
        public string SignatureAlgorithm;          // Algoritmo Di Firma
		//public SubjectCommonName SubjectCommonName;
		public SubjectInfo SubjectInfo;
        public TSInfo[] SignatureTimeStampInfo;
        public bool isCountersigner;
        public SignerInfo[] counterSignatures;
        public DateTime SigningTime;
	}

    [Serializable()]
	public class PKCS7Document 
	{
		public int Level;        
		public string DocumentFileName;
        public string SignAlgorithm;          // Algoritmo usato per firmare il documento.
        public string SignHash;              // Hash del documento Firmato.
		public SignerInfo[] SignersInfo;
	}
     
	// Returned WS information
    [Serializable()]
	public class VerifySignatureResult
	{
		public int StatusCode;
		public string StatusDescription;
		public string FinalDocumentName;
		public bool CRLOnlineCheck;
		public PKCS7Document [] PKCS7Documents;
        public TSInfo[] DocumentTimeStampInfo;
	}
}
