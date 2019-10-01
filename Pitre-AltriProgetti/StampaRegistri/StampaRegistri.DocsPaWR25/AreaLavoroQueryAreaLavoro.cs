using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class AreaLavoroQueryAreaLavoro : MarshalByRefObject
	{
		private Utente utenteField;

		private Ruolo ruoloField;

		private AreaLavoroTipoOggetto tipoObjField;

		private AreaLavoroTipoDocumento tipoDocField;

		private AreaLavoroTipoFascicolo tipoFascField;

		public Utente utente
		{
			get
			{
				return this.utenteField;
			}
			set
			{
				this.utenteField = value;
			}
		}

		public Ruolo ruolo
		{
			get
			{
				return this.ruoloField;
			}
			set
			{
				this.ruoloField = value;
			}
		}

		public AreaLavoroTipoOggetto tipoObj
		{
			get
			{
				return this.tipoObjField;
			}
			set
			{
				this.tipoObjField = value;
			}
		}

		public AreaLavoroTipoDocumento tipoDoc
		{
			get
			{
				return this.tipoDocField;
			}
			set
			{
				this.tipoDocField = value;
			}
		}

		public AreaLavoroTipoFascicolo tipoFasc
		{
			get
			{
				return this.tipoFascField;
			}
			set
			{
				this.tipoFascField = value;
			}
		}
	}
}
