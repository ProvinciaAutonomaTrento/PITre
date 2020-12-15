using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class AreaLavoro
	{
		private int totalRecsField;

		private object[] listaField;

		public int TotalRecs
		{
			get
			{
				return this.totalRecsField;
			}
			set
			{
				this.totalRecsField = value;
			}
		}

		[XmlArrayItem(typeof(Fascicolo)), XmlArrayItem(typeof(InfoDocumento))]
		public object[] lista
		{
			get
			{
				return this.listaField;
			}
			set
			{
				this.listaField = value;
			}
		}
	}
}
