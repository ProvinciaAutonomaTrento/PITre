using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class UserLogin
	{
		private string userNameField;

		private string passwordField;

		private string idAmministrazioneField;

		private string dominioField;

		private string dSTField;

		private bool updateField;

		private string systemIDField;

		private string iPAddressField;

		public string UserName
		{
			get
			{
				return this.userNameField;
			}
			set
			{
				this.userNameField = value;
			}
		}

		public string Password
		{
			get
			{
				return this.passwordField;
			}
			set
			{
				this.passwordField = value;
			}
		}

		public string IdAmministrazione
		{
			get
			{
				return this.idAmministrazioneField;
			}
			set
			{
				this.idAmministrazioneField = value;
			}
		}

		public string Dominio
		{
			get
			{
				return this.dominioField;
			}
			set
			{
				this.dominioField = value;
			}
		}

		public string DST
		{
			get
			{
				return this.dSTField;
			}
			set
			{
				this.dSTField = value;
			}
		}

		public bool Update
		{
			get
			{
				return this.updateField;
			}
			set
			{
				this.updateField = value;
			}
		}

		public string SystemID
		{
			get
			{
				return this.systemIDField;
			}
			set
			{
				this.systemIDField = value;
			}
		}

		public string IPAddress
		{
			get
			{
				return this.iPAddressField;
			}
			set
			{
				this.iPAddressField = value;
			}
		}
	}
}
