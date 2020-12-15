using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class VerifySignatureResult
	{
		private int statusCodeField;

		private string statusDescriptionField;

		private string finalDocumentNameField;

		private bool cRLOnlineCheckField;

		private PKCS7Document[] pKCS7DocumentsField;

		public int StatusCode
		{
			get
			{
				return this.statusCodeField;
			}
			set
			{
				this.statusCodeField = value;
			}
		}

		public string StatusDescription
		{
			get
			{
				return this.statusDescriptionField;
			}
			set
			{
				this.statusDescriptionField = value;
			}
		}

		public string FinalDocumentName
		{
			get
			{
				return this.finalDocumentNameField;
			}
			set
			{
				this.finalDocumentNameField = value;
			}
		}

		public bool CRLOnlineCheck
		{
			get
			{
				return this.cRLOnlineCheckField;
			}
			set
			{
				this.cRLOnlineCheckField = value;
			}
		}

		public PKCS7Document[] PKCS7Documents
		{
			get
			{
				return this.pKCS7DocumentsField;
			}
			set
			{
				this.pKCS7DocumentsField = value;
			}
		}
	}
}
