using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class UnitaOrganizzativa : Corrispondente
	{
		private string codiceField;

		private string indirizzoField;

		private string livelloField;

		private string serverSMTPField;

		private string portaSMTPField;

		private bool interoperanteField;

		private string codiceAOOField;

		private string codiceAmmField;

		private string codiceIstatField;

		private UnitaOrganizzativa parentField;

		private string idUORootField;

		private Registro[] registriField;

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

		public string livello
		{
			get
			{
				return this.livelloField;
			}
			set
			{
				this.livelloField = value;
			}
		}

		public string serverSMTP
		{
			get
			{
				return this.serverSMTPField;
			}
			set
			{
				this.serverSMTPField = value;
			}
		}

		public string portaSMTP
		{
			get
			{
				return this.portaSMTPField;
			}
			set
			{
				this.portaSMTPField = value;
			}
		}

		public bool interoperante
		{
			get
			{
				return this.interoperanteField;
			}
			set
			{
				this.interoperanteField = value;
			}
		}

		public string codiceAOO
		{
			get
			{
				return this.codiceAOOField;
			}
			set
			{
				this.codiceAOOField = value;
			}
		}

		public string codiceAmm
		{
			get
			{
				return this.codiceAmmField;
			}
			set
			{
				this.codiceAmmField = value;
			}
		}

		public string codiceIstat
		{
			get
			{
				return this.codiceIstatField;
			}
			set
			{
				this.codiceIstatField = value;
			}
		}

		public UnitaOrganizzativa parent
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

		public string idUORoot
		{
			get
			{
				return this.idUORootField;
			}
			set
			{
				this.idUORootField = value;
			}
		}

		public Registro[] registri
		{
			get
			{
				return this.registriField;
			}
			set
			{
				this.registriField = value;
			}
		}
	}
}
