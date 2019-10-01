using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.DiagrammaStato
{
	public class DiagrammaStato
	{
		public int SYSTEM_ID = 0;
		public string DESCRIZIONE = "";
		public int ID_AMM = 0;

		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.DiagrammaStato.Stato))]
		public ArrayList STATI = new ArrayList();

		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.DiagrammaStato.Passo))]
		public ArrayList PASSI = new ArrayList();		
	}
}
