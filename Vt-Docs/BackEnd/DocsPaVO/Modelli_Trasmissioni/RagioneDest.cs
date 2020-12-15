using System;
using System.Xml.Serialization;

namespace DocsPaVO.Modelli_Trasmissioni
{
	public class RagioneDest
	{
		public string RAGIONE;
		public string CHA_TIPO_RAGIONE;


		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.Modelli_Trasmissioni.MittDest))]
		public System.Collections.ArrayList DESTINATARI = new System.Collections.ArrayList();	
	}
}
