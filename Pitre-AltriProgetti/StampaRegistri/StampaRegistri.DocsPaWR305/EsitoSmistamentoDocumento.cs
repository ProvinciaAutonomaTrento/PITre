using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class EsitoSmistamentoDocumento
	{
		private string iDPeopleDestinatarioField;

		private string iDCorrGlobaleDestinatarioField;

		private string denominazioneDestinatarioField;

		private int codiceEsitoSmistamentoField;

		private string descrizioneEsitoSmistamentoField;

		public string IDPeopleDestinatario
		{
			get
			{
				return this.iDPeopleDestinatarioField;
			}
			set
			{
				this.iDPeopleDestinatarioField = value;
			}
		}

		public string IDCorrGlobaleDestinatario
		{
			get
			{
				return this.iDCorrGlobaleDestinatarioField;
			}
			set
			{
				this.iDCorrGlobaleDestinatarioField = value;
			}
		}

		public string DenominazioneDestinatario
		{
			get
			{
				return this.denominazioneDestinatarioField;
			}
			set
			{
				this.denominazioneDestinatarioField = value;
			}
		}

		public int CodiceEsitoSmistamento
		{
			get
			{
				return this.codiceEsitoSmistamentoField;
			}
			set
			{
				this.codiceEsitoSmistamentoField = value;
			}
		}

		public string DescrizioneEsitoSmistamento
		{
			get
			{
				return this.descrizioneEsitoSmistamentoField;
			}
			set
			{
				this.descrizioneEsitoSmistamentoField = value;
			}
		}
	}
}
