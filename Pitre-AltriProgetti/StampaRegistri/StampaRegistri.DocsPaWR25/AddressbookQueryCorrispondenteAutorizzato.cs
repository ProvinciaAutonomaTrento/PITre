using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class AddressbookQueryCorrispondenteAutorizzato
	{
		private AddressbookQueryCorrispondente queryCorrispondenteField;

		private RagioneTrasmissione ragioneField;

		private Ruolo ruoloField;

		private TrasmissioneTipoOggetto tipoOggettoField;

		private string idRegistroField;

		private string idNodoTitolarioField;

		public AddressbookQueryCorrispondente queryCorrispondente
		{
			get
			{
				return this.queryCorrispondenteField;
			}
			set
			{
				this.queryCorrispondenteField = value;
			}
		}

		public RagioneTrasmissione ragione
		{
			get
			{
				return this.ragioneField;
			}
			set
			{
				this.ragioneField = value;
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

		public TrasmissioneTipoOggetto tipoOggetto
		{
			get
			{
				return this.tipoOggettoField;
			}
			set
			{
				this.tipoOggettoField = value;
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

		public string idNodoTitolario
		{
			get
			{
				return this.idNodoTitolarioField;
			}
			set
			{
				this.idNodoTitolarioField = value;
			}
		}
	}
}
