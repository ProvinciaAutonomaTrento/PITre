using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class ParametriRicercaRubrica : MarshalByRefObject
	{
		private string parentField;

		private string codiceField;

		private string descrizioneField;

		private string cittaField;

		private bool doUoField;

		private bool doListeField;

		private bool doRuoliField;

		private bool doUtentiField;

		private AddressbookTipoUtente tipoIEField;

		private RubricaCallType calltypeField;

		private RubricaCallerIdentity callerField;

		public string parent
		{
			get
			{
				return this.parentField;
			}
			set
			{
				this.parentField = value;
			}
		}

		public string codice
		{
			get
			{
				return this.codiceField;
			}
			set
			{
				this.codiceField = value;
			}
		}

		public string descrizione
		{
			get
			{
				return this.descrizioneField;
			}
			set
			{
				this.descrizioneField = value;
			}
		}

		public string citta
		{
			get
			{
				return this.cittaField;
			}
			set
			{
				this.cittaField = value;
			}
		}

		public bool doUo
		{
			get
			{
				return this.doUoField;
			}
			set
			{
				this.doUoField = value;
			}
		}

		public bool doListe
		{
			get
			{
				return this.doListeField;
			}
			set
			{
				this.doListeField = value;
			}
		}

		public bool doRuoli
		{
			get
			{
				return this.doRuoliField;
			}
			set
			{
				this.doRuoliField = value;
			}
		}

		public bool doUtenti
		{
			get
			{
				return this.doUtentiField;
			}
			set
			{
				this.doUtentiField = value;
			}
		}

		public AddressbookTipoUtente tipoIE
		{
			get
			{
				return this.tipoIEField;
			}
			set
			{
				this.tipoIEField = value;
			}
		}

		public RubricaCallType calltype
		{
			get
			{
				return this.calltypeField;
			}
			set
			{
				this.calltypeField = value;
			}
		}

		public RubricaCallerIdentity caller
		{
			get
			{
				return this.callerField;
			}
			set
			{
				this.callerField = value;
			}
		}
	}
}
