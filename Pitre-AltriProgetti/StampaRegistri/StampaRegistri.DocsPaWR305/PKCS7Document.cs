using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class PKCS7Document
	{
		private int levelField;

		private string documentFileNameField;

		private SignerInfo[] signersInfoField;

		public int Level
		{
			get
			{
				return this.levelField;
			}
			set
			{
				this.levelField = value;
			}
		}

		public string DocumentFileName
		{
			get
			{
				return this.documentFileNameField;
			}
			set
			{
				this.documentFileNameField = value;
			}
		}

		[XmlArrayItem(IsNullable = false)]
		public SignerInfo[] SignersInfo
		{
			get
			{
				return this.signersInfoField;
			}
			set
			{
				this.signersInfoField = value;
			}
		}
	}
}
