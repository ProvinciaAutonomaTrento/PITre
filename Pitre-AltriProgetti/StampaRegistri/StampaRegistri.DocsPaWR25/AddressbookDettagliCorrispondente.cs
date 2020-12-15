using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class AddressbookDettagliCorrispondente : MarshalByRefObject
	{
		private string indirizzoField;

		private string cittaField;

		private string capField;

		private string provinciaField;

		private string nazioneField;

		private string telefonoField;

		private string telefono2Field;

		private string faxField;

		private string codiceFiscaleField;

		private string noteField;

		public string indirizzo
		{
			get
			{
				return this.indirizzoField;
			}
			set
			{
				this.indirizzoField = value;
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

		public string cap
		{
			get
			{
				return this.capField;
			}
			set
			{
				this.capField = value;
			}
		}

		public string provincia
		{
			get
			{
				return this.provinciaField;
			}
			set
			{
				this.provinciaField = value;
			}
		}

		public string nazione
		{
			get
			{
				return this.nazioneField;
			}
			set
			{
				this.nazioneField = value;
			}
		}

		public string telefono
		{
			get
			{
				return this.telefonoField;
			}
			set
			{
				this.telefonoField = value;
			}
		}

		public string telefono2
		{
			get
			{
				return this.telefono2Field;
			}
			set
			{
				this.telefono2Field = value;
			}
		}

		public string fax
		{
			get
			{
				return this.faxField;
			}
			set
			{
				this.faxField = value;
			}
		}

		public string codiceFiscale
		{
			get
			{
				return this.codiceFiscaleField;
			}
			set
			{
				this.codiceFiscaleField = value;
			}
		}

		public string note
		{
			get
			{
				return this.noteField;
			}
			set
			{
				this.noteField = value;
			}
		}
	}
}
