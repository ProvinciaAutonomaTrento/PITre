using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.DiagrammaStato
{
	public class Passo
	{
		public int ID_DIAGRAMMA;
		public Stato STATO_PADRE;
		public string ID_STATO_AUTOMATICO;
		public string DESCRIZIONE_STATO_AUTOMATICO = "";

		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.DiagrammaStato.Stato))]
		public ArrayList SUCCESSIVI = new ArrayList();
	}
}
