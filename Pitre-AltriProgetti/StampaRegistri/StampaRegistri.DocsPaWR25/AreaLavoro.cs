using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), DesignerCategory("code"), DebuggerStepThrough, XmlType(Namespace = "http://localhost")]
	[Serializable]
	public class AreaLavoro : MarshalByRefObject
	{
		private object[] listaField;

		[XmlArrayItem(typeof(InfoDocumento)), XmlArrayItem(typeof(Fascicolo))]
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
