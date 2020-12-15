using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

namespace DocsPaVO.Smistamento
{
	/// <summary>
	/// Definizione oggetto UnitaOrganizzativa 
	/// relativo alla funzionalità di smistamento documenti.
	/// </summary>
	public class UOSmistamento
	{
		public string ID=string.Empty;
		public string Codice=string.Empty;
		public string Descrizione=string.Empty;

		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.Smistamento.RuoloSmistamento))]
		public ArrayList Ruoli=new ArrayList();


        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.Smistamento.UOSmistamento))]
        public ArrayList UoInferiori = new ArrayList();

        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.Smistamento.UOSmistamento))]
        public ArrayList UoSmistaTrasAutomatica = new ArrayList();

		public bool FlagCompetenza=false;
		public bool FlagConoscenza=false;
        public bool Selezionata = false;

        public string ragioneTrasmRapida = string.Empty;
        public bool modelloNoNotify = false;
	}
}
