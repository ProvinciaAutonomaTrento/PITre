using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento.Internal
{
//	// Subject fields
//	public struct SubjectCommonName
//	{
//		public string CN;
//		public string CodiceFiscale;
//		public string CertId;
//	}

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
		public string SignatureAlgorithm;          // Algoritmo firma (OID)
		public DateTime ValidFromDate;             // Valido da
		public DateTime ValidToDate;               // Valido fino a
        public DateTime RevocationDate;             // DataRevoca
		public string SubjectName;                 // Soggetto
		public string IssuerName;                  // Nome della CA emittente
		public string ThumbPrint;                  // Impronta SHA-1
        public string messages;                    //eventuali messaggi
	}
	
	// Signer information
    [Serializable()]
	public struct SignerInfo
	{
		public string ParentSignerCertSN;
		public CertificateInfo CertificateInfo;
		//public SubjectCommonName SubjectCommonName;
		public SubjectInfo SubjectInfo;
        public string messages;                    //eventuali messaggi
	}

    [Serializable()]
	public class PKCS7Document 
	{
		public int Level;        
		public string DocumentFileName;
		public SignerInfo[] SignersInfo;
        public string messages;                    //eventuali messaggi
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
	}
}
