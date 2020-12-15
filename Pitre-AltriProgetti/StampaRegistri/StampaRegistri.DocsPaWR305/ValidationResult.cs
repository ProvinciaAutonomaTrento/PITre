using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), XmlType(Namespace = "http://localhost")]
	[Serializable]
	public enum ValidationResult
	{
		OK,
		SESSION_EXPIRED,
		SESSION_DROPPED,
		APPLICATION_ERROR
	}
}
