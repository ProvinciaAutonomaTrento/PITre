using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class OggettoCustom
	{
		private int sYSTEM_IDField;

		private string dESCRIZIONEField;

		private string nOMEField;

		private TipoOggetto tIPOField;

		private object[] eLENCO_VALORIField;

		private string oRIZZONTALE_VERTICALEField;

		private string cAMPO_OBBLIGATORIOField;

		private string mULTILINEAField;

		private string nUMERO_DI_LINEEField;

		private string nUMERO_DI_CARATTERIField;

		private string cAMPO_DI_RICERCAField;

		private string tIPO_OPERAZIONEField;

		private string vALORE_DATABASEField;

		private string pOSIZIONEField;

		public int SYSTEM_ID
		{
			get
			{
				return this.sYSTEM_IDField;
			}
			set
			{
				this.sYSTEM_IDField = value;
			}
		}

		public string DESCRIZIONE
		{
			get
			{
				return this.dESCRIZIONEField;
			}
			set
			{
				this.dESCRIZIONEField = value;
			}
		}

		public string NOME
		{
			get
			{
				return this.nOMEField;
			}
			set
			{
				this.nOMEField = value;
			}
		}

		public TipoOggetto TIPO
		{
			get
			{
				return this.tIPOField;
			}
			set
			{
				this.tIPOField = value;
			}
		}

		public object[] ELENCO_VALORI
		{
			get
			{
				return this.eLENCO_VALORIField;
			}
			set
			{
				this.eLENCO_VALORIField = value;
			}
		}

		public string ORIZZONTALE_VERTICALE
		{
			get
			{
				return this.oRIZZONTALE_VERTICALEField;
			}
			set
			{
				this.oRIZZONTALE_VERTICALEField = value;
			}
		}

		public string CAMPO_OBBLIGATORIO
		{
			get
			{
				return this.cAMPO_OBBLIGATORIOField;
			}
			set
			{
				this.cAMPO_OBBLIGATORIOField = value;
			}
		}

		public string MULTILINEA
		{
			get
			{
				return this.mULTILINEAField;
			}
			set
			{
				this.mULTILINEAField = value;
			}
		}

		public string NUMERO_DI_LINEE
		{
			get
			{
				return this.nUMERO_DI_LINEEField;
			}
			set
			{
				this.nUMERO_DI_LINEEField = value;
			}
		}

		public string NUMERO_DI_CARATTERI
		{
			get
			{
				return this.nUMERO_DI_CARATTERIField;
			}
			set
			{
				this.nUMERO_DI_CARATTERIField = value;
			}
		}

		public string CAMPO_DI_RICERCA
		{
			get
			{
				return this.cAMPO_DI_RICERCAField;
			}
			set
			{
				this.cAMPO_DI_RICERCAField = value;
			}
		}

		public string TIPO_OPERAZIONE
		{
			get
			{
				return this.tIPO_OPERAZIONEField;
			}
			set
			{
				this.tIPO_OPERAZIONEField = value;
			}
		}

		public string VALORE_DATABASE
		{
			get
			{
				return this.vALORE_DATABASEField;
			}
			set
			{
				this.vALORE_DATABASEField = value;
			}
		}

		public string POSIZIONE
		{
			get
			{
				return this.pOSIZIONEField;
			}
			set
			{
				this.pOSIZIONEField = value;
			}
		}
	}
}
