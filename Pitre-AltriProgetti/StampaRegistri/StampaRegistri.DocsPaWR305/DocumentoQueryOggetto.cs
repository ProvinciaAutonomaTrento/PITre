using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class DocumentoQueryOggetto
	{
		private object[] idRegistriField;

		private string idAmministrazioneField;

		private string queryDescrizioneField;

		public object[] idRegistri
		{
			get
			{
				return this.idRegistriField;
			}
			set
			{
				this.idRegistriField = value;
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

		public string queryDescrizione
		{
			get
			{
				return this.queryDescrizioneField;
			}
			set
			{
				this.queryDescrizioneField = value;
			}
		}
	}
}
