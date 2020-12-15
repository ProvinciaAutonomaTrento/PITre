using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class InfoDocumento
	{
		private string idProfileField;

		private string docNumberField;

		private string numProtField;

		private string[] mittDestField;

		private string oggettoField;

		private string dataAperturaField;

		private string tipoProtoField;

		private string codRegistroField;

		private string idRegistroField;

		private string segnaturaField;

		private string daProtocollareField;

		private string evidenzaField;

		private string dataAnnullatoField;

		public string idProfile
		{
			get
			{
				return this.idProfileField;
			}
			set
			{
				this.idProfileField = value;
			}
		}

		public string docNumber
		{
			get
			{
				return this.docNumberField;
			}
			set
			{
				this.docNumberField = value;
			}
		}

		public string numProt
		{
			get
			{
				return this.numProtField;
			}
			set
			{
				this.numProtField = value;
			}
		}

		public string[] mittDest
		{
			get
			{
				return this.mittDestField;
			}
			set
			{
				this.mittDestField = value;
			}
		}

		public string oggetto
		{
			get
			{
				return this.oggettoField;
			}
			set
			{
				this.oggettoField = value;
			}
		}

		public string dataApertura
		{
			get
			{
				return this.dataAperturaField;
			}
			set
			{
				this.dataAperturaField = value;
			}
		}

		public string tipoProto
		{
			get
			{
				return this.tipoProtoField;
			}
			set
			{
				this.tipoProtoField = value;
			}
		}

		public string codRegistro
		{
			get
			{
				return this.codRegistroField;
			}
			set
			{
				this.codRegistroField = value;
			}
		}

		public string idRegistro
		{
			get
			{
				return this.idRegistroField;
			}
			set
			{
				this.idRegistroField = value;
			}
		}

		public string segnatura
		{
			get
			{
				return this.segnaturaField;
			}
			set
			{
				this.segnaturaField = value;
			}
		}

		public string daProtocollare
		{
			get
			{
				return this.daProtocollareField;
			}
			set
			{
				this.daProtocollareField = value;
			}
		}

		public string evidenza
		{
			get
			{
				return this.evidenzaField;
			}
			set
			{
				this.evidenzaField = value;
			}
		}

		public string dataAnnullato
		{
			get
			{
				return this.dataAnnullatoField;
			}
			set
			{
				this.dataAnnullatoField = value;
			}
		}
	}
}
