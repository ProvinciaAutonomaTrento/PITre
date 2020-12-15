using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.fascicolazione
{	
	/// <summary>
	/// </summary>
	[XmlType("FascicolazioneClassifica")]
    [Serializable()]
	public class Classifica 
	{
		public string systemId;
		public string codice;
		public string descrizione;
		public bool cha_ReadOnly; //aggiunto elisa 10/08/2005
        public string idTitolario;
        public string bloccaNodiFigli = string.Empty;
        public string contatoreAttivo = string.Empty;
        public string numProtoTit = string.Empty;
	}
}
