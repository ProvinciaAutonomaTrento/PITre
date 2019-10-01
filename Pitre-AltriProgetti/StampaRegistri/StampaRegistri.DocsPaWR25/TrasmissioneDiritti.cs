using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class TrasmissioneDiritti : MarshalByRefObject
	{
		private string accessRightsField;

		private string idAmministrazioneField;

		public string accessRights
		{
			get
			{
				return this.accessRightsField;
			}
			set
			{
				this.accessRightsField = value;
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
	}
}
