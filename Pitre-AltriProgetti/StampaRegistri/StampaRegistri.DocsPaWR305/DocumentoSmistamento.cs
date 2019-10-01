using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class DocumentoSmistamento
	{
		private string iDDocumentoField;

		private string tipoRagioneField;

		private string tipoDocumentoField;

		private string dataCreazioneField;

		private string oggettoField;

		private string mittenteDocumentoField;

		private string[] destinatariDocumentoField;

		private string segnaturaField;

		private FileDocumento immagineDocumentoField;

		public string IDDocumento
		{
			get
			{
				return this.iDDocumentoField;
			}
			set
			{
				this.iDDocumentoField = value;
			}
		}

		public string TipoRagione
		{
			get
			{
				return this.tipoRagioneField;
			}
			set
			{
				this.tipoRagioneField = value;
			}
		}

		public string TipoDocumento
		{
			get
			{
				return this.tipoDocumentoField;
			}
			set
			{
				this.tipoDocumentoField = value;
			}
		}

		public string DataCreazione
		{
			get
			{
				return this.dataCreazioneField;
			}
			set
			{
				this.dataCreazioneField = value;
			}
		}

		public string Oggetto
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

		public string MittenteDocumento
		{
			get
			{
				return this.mittenteDocumentoField;
			}
			set
			{
				this.mittenteDocumentoField = value;
			}
		}

		public string[] DestinatariDocumento
		{
			get
			{
				return this.destinatariDocumentoField;
			}
			set
			{
				this.destinatariDocumentoField = value;
			}
		}

		public string Segnatura
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

		public FileDocumento ImmagineDocumento
		{
			get
			{
				return this.immagineDocumentoField;
			}
			set
			{
				this.immagineDocumentoField = value;
			}
		}
	}
}
