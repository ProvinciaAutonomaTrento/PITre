using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlInclude(typeof(Trasmissione)), XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class TrasmissioneOggettoTrasm : MarshalByRefObject
	{
		private InfoDocumento infoDocumentoField;

		private InfoFascicolo infoFascicoloField;

		public InfoDocumento infoDocumento
		{
			get
			{
				return this.infoDocumentoField;
			}
			set
			{
				this.infoDocumentoField = value;
			}
		}

		public InfoFascicolo infoFascicolo
		{
			get
			{
				return this.infoFascicoloField;
			}
			set
			{
				this.infoFascicoloField = value;
			}
		}
	}
}
