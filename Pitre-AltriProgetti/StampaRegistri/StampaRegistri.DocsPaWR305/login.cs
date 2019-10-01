using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class login : MarshalByRefObject
	{
		private string userNameField;

		private string passwordField;

		private string idAmministrazioneField;

		private string dominioField;

		private bool updateField;

		public string userName
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

		public string password
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

		public string idAmministrazione
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

		public string dominio
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

		public bool update
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
	}
}
