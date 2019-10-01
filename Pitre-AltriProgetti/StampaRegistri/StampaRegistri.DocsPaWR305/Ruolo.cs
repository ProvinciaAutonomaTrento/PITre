using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class Ruolo : Corrispondente
	{
		private string livelloField;

		private string codiceField;

		private string idGruppoField;

		private string codiceIstatField;

		private TipoRuolo tipoRuoloField;

		private UnitaOrganizzativa uoField;

		private Funzione[] funzioniField;

		private Registro[] registriField;

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

		public string idGruppo
		{
			get
			{
				return this.idGruppoField;
			}
			set
			{
				this.idGruppoField = value;
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

		public TipoRuolo tipoRuolo
		{
			get
			{
				return this.tipoRuoloField;
			}
			set
			{
				this.tipoRuoloField = value;
			}
		}

		public UnitaOrganizzativa uo
		{
			get
			{
				return this.uoField;
			}
			set
			{
				this.uoField = value;
			}
		}

		public Funzione[] funzioni
		{
			get
			{
				return this.funzioniField;
			}
			set
			{
				this.funzioniField = value;
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
