using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class CertificateInfo
	{
		private int revocationStatusField;

		private string revocationStatusDescriptionField;

		private string serialNumberField;

		private string signatureAlgorithmField;

		private DateTime validFromDateField;

		private DateTime validToDateField;

		private string subjectNameField;

		private string issuerNameField;

		private string thumbPrintField;

		public int RevocationStatus
		{
			get
			{
				return this.revocationStatusField;
			}
			set
			{
				this.revocationStatusField = value;
			}
		}

		public string RevocationStatusDescription
		{
			get
			{
				return this.revocationStatusDescriptionField;
			}
			set
			{
				this.revocationStatusDescriptionField = value;
			}
		}

		public string SerialNumber
		{
			get
			{
				return this.serialNumberField;
			}
			set
			{
				this.serialNumberField = value;
			}
		}

		public string SignatureAlgorithm
		{
			get
			{
				return this.signatureAlgorithmField;
			}
			set
			{
				this.signatureAlgorithmField = value;
			}
		}

		public DateTime ValidFromDate
		{
			get
			{
				return this.validFromDateField;
			}
			set
			{
				this.validFromDateField = value;
			}
		}

		public DateTime ValidToDate
		{
			get
			{
				return this.validToDateField;
			}
			set
			{
				this.validToDateField = value;
			}
		}

		public string SubjectName
		{
			get
			{
				return this.subjectNameField;
			}
			set
			{
				this.subjectNameField = value;
			}
		}

		public string IssuerName
		{
			get
			{
				return this.issuerNameField;
			}
			set
			{
				this.issuerNameField = value;
			}
		}

		public string ThumbPrint
		{
			get
			{
				return this.thumbPrintField;
			}
			set
			{
				this.thumbPrintField = value;
			}
		}
	}
}
