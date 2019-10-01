using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlInclude(typeof(Allegato)), XmlInclude(typeof(Documento)), XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class FileRequest
	{
		private string dataInserimentoField;

		private string descrizioneField;

		private string docNumberField;

		private string docServerLocField;

		private string pathField;

		private string fileNameField;

		private string idPeopleField;

		private string versionIdField;

		private string versionField;

		private string subVersionField;

		private string versionLabelField;

		private string fileSizeField;

		private Applicazione applicazioneField;

		private Firmatario[] firmatariField;

		private bool daAggiornareFirmatariField;

		public string dataInserimento
		{
			get
			{
				return this.dataInserimentoField;
			}
			set
			{
				this.dataInserimentoField = value;
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

		public string docServerLoc
		{
			get
			{
				return this.docServerLocField;
			}
			set
			{
				this.docServerLocField = value;
			}
		}

		public string path
		{
			get
			{
				return this.pathField;
			}
			set
			{
				this.pathField = value;
			}
		}

		public string fileName
		{
			get
			{
				return this.fileNameField;
			}
			set
			{
				this.fileNameField = value;
			}
		}

		public string idPeople
		{
			get
			{
				return this.idPeopleField;
			}
			set
			{
				this.idPeopleField = value;
			}
		}

		public string versionId
		{
			get
			{
				return this.versionIdField;
			}
			set
			{
				this.versionIdField = value;
			}
		}

		public string version
		{
			get
			{
				return this.versionField;
			}
			set
			{
				this.versionField = value;
			}
		}

		public string subVersion
		{
			get
			{
				return this.subVersionField;
			}
			set
			{
				this.subVersionField = value;
			}
		}

		public string versionLabel
		{
			get
			{
				return this.versionLabelField;
			}
			set
			{
				this.versionLabelField = value;
			}
		}

		public string fileSize
		{
			get
			{
				return this.fileSizeField;
			}
			set
			{
				this.fileSizeField = value;
			}
		}

		public Applicazione applicazione
		{
			get
			{
				return this.applicazioneField;
			}
			set
			{
				this.applicazioneField = value;
			}
		}

		public Firmatario[] firmatari
		{
			get
			{
				return this.firmatariField;
			}
			set
			{
				this.firmatariField = value;
			}
		}

		public bool daAggiornareFirmatari
		{
			get
			{
				return this.daAggiornareFirmatariField;
			}
			set
			{
				this.daAggiornareFirmatariField = value;
			}
		}
	}
}
