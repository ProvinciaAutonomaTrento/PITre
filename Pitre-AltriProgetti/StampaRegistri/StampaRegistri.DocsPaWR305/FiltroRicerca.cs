using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class FiltroRicerca
	{
		private string argomentoField;

		private string valoreField;

		private FiltriDocumento listaFiltriDocumentoField;

		private FiltriTrasmissione listaFiltriTrasmissioneField;

		private FiltriTrasmissioneNascosti listaFiltriTrasmissioneNascostiField;

		private FiltriFascicolazione listaFiltriFascicoloField;

		public string argomento
		{
			get
			{
				return this.argomentoField;
			}
			set
			{
				this.argomentoField = value;
			}
		}

		public string valore
		{
			get
			{
				return this.valoreField;
			}
			set
			{
				this.valoreField = value;
			}
		}

		public FiltriDocumento listaFiltriDocumento
		{
			get
			{
				return this.listaFiltriDocumentoField;
			}
			set
			{
				this.listaFiltriDocumentoField = value;
			}
		}

		public FiltriTrasmissione listaFiltriTrasmissione
		{
			get
			{
				return this.listaFiltriTrasmissioneField;
			}
			set
			{
				this.listaFiltriTrasmissioneField = value;
			}
		}

		public FiltriTrasmissioneNascosti listaFiltriTrasmissioneNascosti
		{
			get
			{
				return this.listaFiltriTrasmissioneNascostiField;
			}
			set
			{
				this.listaFiltriTrasmissioneNascostiField = value;
			}
		}

		public FiltriFascicolazione listaFiltriFascicolo
		{
			get
			{
				return this.listaFiltriFascicoloField;
			}
			set
			{
				this.listaFiltriFascicoloField = value;
			}
		}
	}
}
