using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class OrgUO
	{
		private string iDCorrGlobaleField;

		private string codiceField;

		private string codiceRubricaField;

		private string descrizioneField;

		private string livelloField;

		private string iDAmministrazioneField;

		private string codiceRegistroInteropField;

		private string ruoliField;

		private string sottoUoField;

		private string iDParentField;

		public string IDCorrGlobale
		{
			get
			{
				return this.iDCorrGlobaleField;
			}
			set
			{
				this.iDCorrGlobaleField = value;
			}
		}

		public string Codice
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

		public string CodiceRubrica
		{
			get
			{
				return this.codiceRubricaField;
			}
			set
			{
				this.codiceRubricaField = value;
			}
		}

		public string Descrizione
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

		public string Livello
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

		public string IDAmministrazione
		{
			get
			{
				return this.iDAmministrazioneField;
			}
			set
			{
				this.iDAmministrazioneField = value;
			}
		}

		public string CodiceRegistroInterop
		{
			get
			{
				return this.codiceRegistroInteropField;
			}
			set
			{
				this.codiceRegistroInteropField = value;
			}
		}

		public string Ruoli
		{
			get
			{
				return this.ruoliField;
			}
			set
			{
				this.ruoliField = value;
			}
		}

		public string SottoUo
		{
			get
			{
				return this.sottoUoField;
			}
			set
			{
				this.sottoUoField = value;
			}
		}

		public string IDParent
		{
			get
			{
				return this.iDParentField;
			}
			set
			{
				this.iDParentField = value;
			}
		}
	}
}
