using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class DatiTrasmissioneDocumento
	{
		private string iDDocumentoField;

		private bool trasmissioneConWorkflowField;

		private string iDTrasmissioneField;

		private string iDTrasmissioneSingolaField;

		private string iDTrasmissioneUtenteField;

		private string noteGeneraliField;

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

		public bool TrasmissioneConWorkflow
		{
			get
			{
				return this.trasmissioneConWorkflowField;
			}
			set
			{
				this.trasmissioneConWorkflowField = value;
			}
		}

		public string IDTrasmissione
		{
			get
			{
				return this.iDTrasmissioneField;
			}
			set
			{
				this.iDTrasmissioneField = value;
			}
		}

		public string IDTrasmissioneSingola
		{
			get
			{
				return this.iDTrasmissioneSingolaField;
			}
			set
			{
				this.iDTrasmissioneSingolaField = value;
			}
		}

		public string IDTrasmissioneUtente
		{
			get
			{
				return this.iDTrasmissioneUtenteField;
			}
			set
			{
				this.iDTrasmissioneUtenteField = value;
			}
		}

		public string NoteGenerali
		{
			get
			{
				return this.noteGeneraliField;
			}
			set
			{
				this.noteGeneraliField = value;
			}
		}
	}
}
