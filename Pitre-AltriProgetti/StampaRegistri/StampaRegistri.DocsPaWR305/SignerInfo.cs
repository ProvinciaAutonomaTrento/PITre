using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class SignerInfo
	{
		private string parentSignerCertSNField;

		private CertificateInfo certificateInfoField;

		private SubjectCommonName subjectCommonNameField;

		private SubjectDescription subjectDescriptionField;

		public string ParentSignerCertSN
		{
			get
			{
				return this.parentSignerCertSNField;
			}
			set
			{
				this.parentSignerCertSNField = value;
			}
		}

		public CertificateInfo CertificateInfo
		{
			get
			{
				return this.certificateInfoField;
			}
			set
			{
				this.certificateInfoField = value;
			}
		}

		public SubjectCommonName SubjectCommonName
		{
			get
			{
				return this.subjectCommonNameField;
			}
			set
			{
				this.subjectCommonNameField = value;
			}
		}

		public SubjectDescription SubjectDescription
		{
			get
			{
				return this.subjectDescriptionField;
			}
			set
			{
				this.subjectDescriptionField = value;
			}
		}
	}
}
