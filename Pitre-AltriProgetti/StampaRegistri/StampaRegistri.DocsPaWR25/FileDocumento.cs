using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class FileDocumento
	{
		private string nameField;

		private string pathField;

		private string fullNameField;

		private byte[] contentField;

		private int lengthField;

		private string contentTypeField;

		private string estensioneFileField;

		public string name
		{
			get
			{
				return this.nameField;
			}
			set
			{
				this.nameField = value;
			}
		}

		public string path
		{
			get
			{
				return this.pathField;
			}
			set
			{
				this.pathField = value;
			}
		}

		public string fullName
		{
			get
			{
				return this.fullNameField;
			}
			set
			{
				this.fullNameField = value;
			}
		}

		[XmlElement(DataType = "base64Binary")]
		public byte[] content
		{
			get
			{
				return this.contentField;
			}
			set
			{
				this.contentField = value;
			}
		}

		public int length
		{
			get
			{
				return this.lengthField;
			}
			set
			{
				this.lengthField = value;
			}
		}

		public string contentType
		{
			get
			{
				return this.contentTypeField;
			}
			set
			{
				this.contentTypeField = value;
			}
		}

		public string estensioneFile
		{
			get
			{
				return this.estensioneFileField;
			}
			set
			{
				this.estensioneFileField = value;
			}
		}
	}
}
